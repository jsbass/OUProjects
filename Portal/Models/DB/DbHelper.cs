using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Portal.Helpers;
using Portal.Models.GeoJson;
using Portal.Models.Outgoing.Map;

namespace Portal.Models.DB
{
    public static class DbHelper
    {
        public static Entities GetDb()
        {
            return new Entities();
        }

        public static async Task<bool> SaveBuilding(Models.Outgoing.Map.Building building)
        {
            using (var db = DbHelper.GetDb())
            {
                Building dbObject;
                if (building.Id == null)
                {
                    var dbLocation = new Models.DB.Location()
                    {
                        Description = building.Description,
                        MarkerData = JsonConvert.SerializeObject(building.Marker, Formatting.Indented),
                        Name = building.Name
                    };
                    dbObject = new Models.DB.Building
                    {
                        BuildingCode = building.OuCode,
                        GeoData = JsonConvert.SerializeObject(building.Shape, Formatting.Indented),
                    };
                    dbLocation.Building = dbObject;
                    db.Locations.Add(dbLocation);
                }
                else
                {
                    dbObject = db.Buildings.Include(b => b.Location).FirstOrDefault(b => b.fkLocationId == building.Id);
                    if (dbObject == null)
                    {
                        return false;
                    }
                    dbObject.BuildingCode = building.OuCode;
                    dbObject.Location.Description = building.Description;
                    dbObject.Location.GeoData = JsonConvert.SerializeObject(building.Shape, Formatting.Indented);
                    dbObject.Location.MarkerData = JsonConvert.SerializeObject(building.Marker, Formatting.Indented);
                    dbObject.Location.Name = building.Name;
                }
                try
                {
                    await db.SaveChangesAsync();
                    building.Id = dbObject.fkLocationId;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            var cache = new Caching();
            await cache.UpdateAsync<CachedLocationsContainer>("LOCATIONS", container =>
            {
                var current = container.Buildings.FirstOrDefault(b => b.Id == building.Id);
                if (current != null)
                {
                    current.Description = building.Description;
                    current.Marker = building.Marker;
                    current.Shape = building.Shape;
                    current.Name = building.Name;
                    current.OuCode = building.OuCode;
                }
                else
                {
                    container.Buildings.Add(building);
                }
                return Task.FromResult(container);
            }, new TimeSpan(0, 1, 0));
            return true;
        }

        public static async Task<bool> SaveParking(Models.Outgoing.Map.Parking parking)
        {
            using (var db = DbHelper.GetDb())
            {
                Parking dbObject;
                if (parking.Id == null)
                {
                    var dbLocation = new Location()
                    {
                        Description = parking.Description,
                        MarkerData = JsonConvert.SerializeObject(parking.Marker, Formatting.Indented),
                        Name = parking.Name
                    };
                    dbObject = new Parking()
                    {
                        ParkingCode = parking.OuCode,
                        GeoData = JsonConvert.SerializeObject(parking.Shape, Formatting.Indented),
                    };
                    dbLocation.Parking = dbObject;
                    db.Locations.Add(dbLocation);
                }
                else
                {
                    dbObject = db.Parkings.Include(b => b.Location).FirstOrDefault(b => b.fkLocationId == parking.Id);
                    if (dbObject == null)
                    {
                        return false;
                    }
                    dbObject.ParkingCode = parking.OuCode;
                    dbObject.Location.Description = parking.Description;
                    dbObject.Location.GeoData = JsonConvert.SerializeObject(parking.Shape, Formatting.Indented);
                    dbObject.Location.MarkerData = JsonConvert.SerializeObject(parking.Marker, Formatting.Indented);
                    dbObject.Location.Name = parking.Name;
                }
                try
                {
                    await db.SaveChangesAsync();
                    parking.Id = dbObject.fkLocationId;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            var cache = new Caching();
            await cache.UpdateAsync<CachedLocationsContainer>("LOCATIONS", container =>
            {
                var current = container.Parkings.FirstOrDefault(b => b.Id == parking.Id);
                if (current != null)
                {
                    current.Description = parking.Description;
                    current.Marker = parking.Marker;
                    current.Shape = parking.Shape;
                    current.Name = parking.Name;
                    current.OuCode = parking.OuCode;
                }
                else
                {
                    container.Parkings.Add(parking);
                }
                return Task.FromResult(container);
            }, new TimeSpan(0, 1, 0));
            return true;
        }

        public static async Task UpdateLocationsContainer()
        {
            await GetOrUpdateLocationsContainer(true);
        }

        public static async Task<CachedLocationsContainer> GetLocationsContainer()
        {
            return await GetOrUpdateLocationsContainer(false);
        }

        private static async Task<CachedLocationsContainer> GetOrUpdateLocationsContainer(bool forceRefresh)
        {
            var caching = new Caching();
            var key = "LOCATIONS";
            return await caching.GetOrAddAsync<CachedLocationsContainer>(key, async () =>
            {
                using (var db = GetDb())
                {
                    var buildings = await db.Buildings.Include(b => b.Location).ToListAsync();
                    var parking = await db.Parkings.Include(p => p.Location).Include(p => p.ParkingTypes).ToListAsync();

                    var container = new CachedLocationsContainer();
                    container.Parkings = parking.Select(p => new Models.Outgoing.Map.Parking()
                    {
                        Description = p.Location.Description,
                        HandicappedCount = p.HandicapCount,
                        Id = p.fkLocationId,
                        Marker = JsonConvert.DeserializeObject<Point>(p.Location.MarkerData),
                        Name = p.Location.Name,
                        OuCode = p.ParkingCode,
                        ParkingTypes = p.ParkingTypes.Select(pt => pt.Name).ToList(),
                        Shape = JsonConvert.DeserializeObject<Polygon>(p.Location.GeoData),
                        Tags = new List<string>(),
                        ImgUrl = p.Location.ImgUrl
                    }).ToList();
                    container.Buildings = buildings.Select(b => new Models.Outgoing.Map.Building()
                    {
                        Description = b.Location.Description,
                        Id = b.fkLocationId,
                        Marker = JsonConvert.DeserializeObject<Point>(b.Location.MarkerData),
                        Name = b.Location.Name,
                        OuCode = b.BuildingCode,
                        Shape = JsonConvert.DeserializeObject<Polygon>(b.Location.GeoData),
                        Tags = new List<string>(),
                        ImgUrl = b.Location.ImgUrl
                    }).ToList();

                    container.UpdateBounds();

                    return container;
                }
            }, new TimeSpan(0, 1, 0), forceRefresh);
        }
    }

    public partial class Entities
    {
        public async Task<List<Category>> GetCategories(User user)
        {
            var query = Categories.Include(c => c.LinkCategories.Select(lc => lc.Link));

            if (user != null)
            {
                return await query.Where(c => c.UserId == user.UserId || c.UserId == null).ToListAsync();
            }
            else
            {
                return await query.Where(c => c.UserId == null).ToListAsync();
            }
        }
    }
}