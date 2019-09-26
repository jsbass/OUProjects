using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using Portal.Helpers;
using Portal.Models;
using Portal.Models.DB;
using Portal.Models.Incoming.OuMaps;
using Portal.Models.OuMaps;
using Newtonsoft.Json;
using Portal.Models.GeoJson;
using Portal.Models.Outgoing.Map;

namespace Portal.Controllers.Api
{
    public class MapsController : ApiController
    {
        private const int MaxContentLength = 1024*1024*1;
        private static List<string> AllowedImageExtensions = new List<string>()
        {
            ".jpg",
            ".png",
            ".gif"
        };

        [Route("api/map/locations")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLocationAt(double lng, double lat)
        {
            var locContainer = await DbHelper.GetLocationsContainer();
            if (lng < locContainer.Bounds.Left || lng > locContainer.Bounds.Right || lat < locContainer.Bounds.Bottom || lat > locContainer.Bounds.Top)
            {
                return Ok(new { bounds = locContainer.Bounds });
            }
            var p = new PointD(lng, lat);

            return Ok(new {locs = locContainer.Locations.Where(l => l.Contains(p)), bounds = locContainer.Bounds });
        }

        [Route("api/map/locations/shapes")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLocationShapes()
        {
            var container = await DbHelper.GetLocationsContainer();
            return Ok(new OverlayShapesContainer()
            {
                Locations = container.Locations.Select(l => new OverlayShape()
                {
                    Id = l.Id ?? 0,
                    LocationType = l.LocationType,
                    Marker = l.Marker,
                    Shape = l.Shape
                }).ToList()
            });
        }

        [Route("api/map/locations")]
        [HttpGet]
        public async Task<IHttpActionResult> SearchLocations(string search = "")
        {
            if (string.IsNullOrWhiteSpace(search)) return Ok(new List<Models.Outgoing.Map.Location>());
            var locContainer = await DbHelper.GetLocationsContainer();
            return Ok(locContainer.Search(search));
        }

        [Route("api/map/locations/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLocation(int id)
        {
            var container = await DbHelper.GetLocationsContainer();
            var loc = container.Locations.FirstOrDefault(l => l.Id == id);
            if (loc != null)
            {
                return Ok(loc);
            }

            return BadRequest();
        }

        [Route("api/map/locations")]
        [HttpPost]
        public async Task<IHttpActionResult> PostLocation()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var location = JsonConvert.DeserializeObject<Models.Outgoing.Map.Location>(body);

            bool result;
            switch (location.LocationType)
            {
                case LocationTypes.Building:
                    result = await DbHelper.SaveBuilding(JsonConvert.DeserializeObject<Models.Outgoing.Map.Building>(body));
                    break;
                case LocationTypes.Parking:
                    result = await DbHelper.SaveParking(JsonConvert.DeserializeObject<Models.Outgoing.Map.Parking>(body));
                    break;
                case LocationTypes.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (result)
            {
                await DbHelper.UpdateLocationsContainer();
                return Ok();
            }
            else
            {
                return InternalServerError();
            }
        }

        [Route("api/map/locations/{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteLocation(int id)
        {
            using (var db = DbHelper.GetDb())
            {
                var loc = await db.Locations.FirstOrDefaultAsync(l => l.Id == id);
                if (loc != null)
                {
                    db.Locations.Remove(loc);

                    try
                    {
                        await db.SaveChangesAsync();
                        return Ok();
                    }
                    catch (Exception)
                    {
                        return InternalServerError();
                    }
                }

                await DbHelper.UpdateLocationsContainer();
                return Ok();
            }
        }
        
        [Authorize(Roles = "MapEditor,Admin")]
        [Route("api/map/locations/buildings")]
        [HttpPost]
        public async Task<IHttpActionResult> PostBuilding(Models.Outgoing.Map.Building building)
        {
            using (var db = DbHelper.GetDb())
            {
                if (building.Id == null)
                {
                    var dbLocation = new Models.DB.Location()
                    {
                        Description = building.Description,
                        MarkerData = JsonConvert.SerializeObject(building.Marker, Formatting.Indented),
                        Name = building.Name
                    };
                    var dbBuilding = new Models.DB.Building
                    {
                        BuildingCode = building.OuCode,
                        GeoData = JsonConvert.SerializeObject(building.Shape, Formatting.Indented),
                    };
                    dbLocation.Building = dbBuilding;
                    db.Locations.Add(dbLocation);
                }
                else
                {
                    var dbObject = db.Buildings.Include(b => b.Location).FirstOrDefault(b => b.fkLocationId == building.Id);
                    if (dbObject == null)
                    {
                        return Content(HttpStatusCode.BadRequest, $"Building with id {building.Id} not found");
                    }
                    dbObject.BuildingCode = building.OuCode;
                    dbObject.Location.Description = building.Description;
                    dbObject.Location.GeoData = JsonConvert.SerializeObject(building.Shape, Formatting.Indented);
                    dbObject.Location.MarkerData = JsonConvert.SerializeObject(building.Marker, Formatting.Indented);
                    dbObject.Location.Name = building.Name;
                }
                return Ok(await db.SaveChangesAsync());
            }
        }
        
        [Route("api/map/locations/parking")]
        [Authorize(Roles = "MapEditor,Admin")]
        [HttpPost]
        public async Task<IHttpActionResult> PostParking(Models.Outgoing.Map.Parking parking)
        {
            using (var db = DbHelper.GetDb())
            {
                Models.DB.Parking dbObject;
                if (parking.Id == null)
                {
                    var dbLocation = new Models.DB.Location()
                    {
                        Description = parking.Description,
                        MarkerData = JsonConvert.SerializeObject(parking.Marker, Formatting.Indented),
                        Name = parking.Name,
                    };
                    var dbParking = new Models.DB.Parking()
                    {
                        GeoData = JsonConvert.SerializeObject(parking.Shape, Formatting.Indented),
                        ParkingCode = parking.OuCode,
                        SpaceCount = parking.SpaceCount,
                        HandicapCount = parking.HandicappedCount
                    };

                    foreach (var typeName in parking.ParkingTypes)
                    {
                        var parkingType = db.ParkingTypes.FirstOrDefault(pt => pt.Name == typeName);
                        if (parkingType == null)
                        {
                            return Content(HttpStatusCode.BadRequest, $"Parking Type: {typeName} not a valid type");
                        }
                        else
                        {
                            dbParking.ParkingTypes.Add(parkingType);
                        }
                    }
                    dbLocation.Parking = dbParking;
                    db.Locations.Add(dbLocation);
                }
                else
                {
                    dbObject = db.Parkings.Include(p => p.Location).FirstOrDefault(p => p.fkLocationId == parking.Id);
                    if (dbObject == null)
                    {
                        return Content(HttpStatusCode.BadRequest, $"Building with id {parking.Id} not found");
                    }
                    dbObject.ParkingCode = parking.OuCode;
                    dbObject.Location.Description = parking.Description;
                    dbObject.Location.GeoData = JsonConvert.SerializeObject(parking.Shape, Formatting.Indented);
                    dbObject.Location.MarkerData = JsonConvert.SerializeObject(parking.Marker, Formatting.Indented);
                    dbObject.Location.Name = parking.Name;
                    dbObject.SpaceCount = parking.SpaceCount;
                    dbObject.HandicapCount = parking.HandicappedCount;

                    foreach (var typeName in parking.ParkingTypes)
                    {
                        var parkingType = db.ParkingTypes.FirstOrDefault(pt => pt.Name == typeName);
                        if (parkingType == null)
                        {
                            return Content(HttpStatusCode.BadRequest, $"Parking Type: {typeName} not a valid type");
                        }
                        else
                        {
                            dbObject.ParkingTypes.Add(parkingType);
                        }
                    }
                }
                return Ok(await db.SaveChangesAsync());
            }
        }

        #region importation

        [Route("api/maps/import/OU")]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IHttpActionResult> ImportOuMaps()
        {
            var helper = new OuMapHelper();
            var buildingsSearch = await helper.GetBuildings();
            var parkingSearch = await helper.GetParking();
            var buildingDTOs = new List<LocationDTO<Models.Incoming.OuMaps.Building>>();
            var parkingDTOs = new List<LocationDTO<Models.Incoming.OuMaps.Parking>>();
            foreach (var searchItem in buildingsSearch)
            {
                buildingDTOs.Add(new LocationDTO<Models.Incoming.OuMaps.Building>()
                {
                    LocationSearchItem = searchItem,
                    LocationTask = helper.GetLocation<Models.Incoming.OuMaps.Building>(searchItem.Locurl)
                });
            }
            foreach (var searchItem in parkingSearch)
            {
                parkingDTOs.Add(new LocationDTO<Models.Incoming.OuMaps.Parking>()
                {
                    LocationSearchItem = searchItem,
                    LocationTask = helper.GetLocation<Models.Incoming.OuMaps.Parking>(searchItem.Locurl)
                });
            }
            using (var db = DbHelper.GetDb())
            {
                foreach (var buildingDto in buildingDTOs)
                {
                    var buildingEntry = new OuMapDumpBuilding();
                    buildingEntry.displayAs = buildingDto.LocationSearchItem.DisplayAs;
                    var info = await buildingDto.LocationTask;
                    buildingEntry.accessibility = info.Accessibility;
                    buildingEntry.floorplan = info.Floorplan;
                    buildingEntry.geodata = info.Geodata;
                    buildingEntry.loccode = info.Loccode;
                    buildingEntry.locid = info.Locid;
                    buildingEntry.locimg = info.Locimg;
                    buildingEntry.lockeys = info.Lockeys;
                    buildingEntry.locoptions = string.Join(",", info.Locoptions);
                    buildingEntry.loctext = info.Loctext;
                    buildingEntry.loctitle = info.Loctitle;
                    buildingEntry.locurl = info.Locurl;

                    db.OuMapDumpBuildings.Add(buildingEntry);
                }

                foreach (var parkingDto in parkingDTOs)
                {
                    var parkingEntry = new OuMapDumpParking();
                    parkingEntry.displayAs = parkingDto.LocationSearchItem.DisplayAs;
                    var info = await parkingDto.LocationTask;
                    parkingEntry.isparking = info.isParking;
                    parkingEntry.handicappedcount = info.Handicappedcount;
                    parkingEntry.spacecount = info.Spacecount;
                    parkingEntry.parkingtypes = string.Join(",", info.Parkingtypes);
                    parkingEntry.geodata = info.Geodata;
                    parkingEntry.loccode = info.Loccode;
                    parkingEntry.locid = info.Locid;
                    parkingEntry.locimg = info.Locimg;
                    parkingEntry.lockeys = info.Lockeys;
                    parkingEntry.locoptions = string.Join(",", info.Locoptions);
                    parkingEntry.loctext = info.Loctext;
                    parkingEntry.loctitle = info.Loctitle;
                    parkingEntry.locurl = info.Locurl;

                    db.OuMapDumpParkings.Add(parkingEntry);
                }

                try
                {
                    return Ok(await db.SaveChangesAsync());
                }
                catch (DbEntityValidationException e)
                {
                    return Content(HttpStatusCode.InternalServerError, e);
                }
            }
        }

        private class LocationDTO<T> where T : Models.Incoming.OuMaps.Location
        {
            public Task<T> LocationTask { get; set; }
            public SearchItem LocationSearchItem { get; set; }
        }

        [Route("api/map/ou/migrate/building")]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IHttpActionResult> PopulateBuildingTableFromImport()
        {
            using (var db = DbHelper.GetDb())
            {
                var incoming = await db.OuMapDumpBuildings.ToListAsync();

                incoming.ForEach(b =>
                {
                    var customData = JsonConvert.DeserializeObject<OuMapsCustomGeoData>(b.geodata);
                    var marker = new Models.Point()
                    {
                        Position = new Position()
                        {
                            Longitude = customData.Longitude,
                            Latitude = customData.Latitude
                        }
                    };
                    var shape = customData.Polygon;
                    var location = new Models.DB.Location()
                    {
                        Name = b.loctitle,
                        Description = b.loctext,
                        ImgUrl = string.IsNullOrEmpty(b.locimg) ? b.locimg : "http://www.ou.edu" + b.locimg,
                        MarkerData = JsonConvert.SerializeObject(marker, Formatting.Indented),
                        GeoData = JsonConvert.SerializeObject(shape, Formatting.Indented)
                    };
                    location.Building = new Models.DB.Building()
                    {
                        BuildingCode = b.loccode
                    };
                    db.Locations.Add(location);
                });

                return Ok(await db.SaveChangesAsync());
            }
        }

        [Route("api/map/ou/migrate/parking")]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IHttpActionResult> PopulateParkingTableFromImport()
        {
            using (var db = DbHelper.GetDb())
            {
                var incoming = await db.OuMapDumpParkings.ToListAsync();

                incoming.ForEach(p =>
                {
                    var customData = JsonConvert.DeserializeObject<OuMapsCustomGeoData>(p.geodata);
                    var marker = new Models.Point()
                    {
                        Position = new Position()
                        {
                            Longitude = customData.Longitude,
                            Latitude = customData.Latitude
                        }
                    };
                    var shape = customData.Polygon;
                    int spaces, handicap;
                    if (!int.TryParse(p.spacecount, out spaces))
                    {
                        spaces = 0;
                    }
                    if (!int.TryParse(p.handicappedcount, out handicap))
                    {
                        handicap = 0;
                    }
                    var location = new Models.DB.Location()
                    {
                        Name = p.loctitle,
                        Description = p.loctext,
                        ImgUrl = string.IsNullOrEmpty(p.locimg) ? p.locimg : "http://www.ou.edu" + p.locimg,
                        MarkerData = JsonConvert.SerializeObject(marker, Formatting.Indented),
                        GeoData = JsonConvert.SerializeObject(shape, Formatting.Indented)
                    };
                    var parking = new Models.DB.Parking()
                    {
                        ParkingCode = p.loccode,
                        HandicapCount = handicap,
                        SpaceCount = spaces
                    };
                    foreach (var parkingType in p.parkingtypes.Split(','))
                    {
                        var trimmed = parkingType.Trim();
                        var type = db.ParkingTypes.Local.FirstOrDefault(t => t.Name == trimmed);
                        if(type == null)
                        {
                            type = new ParkingType()
                            {
                                Name = trimmed
                            };
                            db.ParkingTypes.Add(type);
                        }
                        parking.ParkingTypes.Add(type);
                    }
                    location.Parking = parking;
                    db.Locations.Add(location);
                });

                try
                {

                    return Ok(await db.SaveChangesAsync());
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.InternalServerError, e);
                }
            }
        }
#endregion
    }
}