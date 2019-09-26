using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Portal.Models.GeoJson;

namespace Portal.Models.Outgoing.Map
{
    public class ParkingContainer
    {
        [JsonProperty("parking")]
        public List<Parking> Parking { get; set; }

        [JsonProperty("parkingTypes")]
        public List<string> ParkingTypes { get; set; }
    }

    public class Parking : Location
    {
        public Parking()
        {
            LocationType = LocationTypes.Parking;
        }

        [JsonProperty("spaceCount")]
        public int SpaceCount { get; set; }

        [JsonProperty("handicappedCount")]
        public int HandicappedCount { get; set; }

        [JsonProperty("parkingTypes")]
        public List<string> ParkingTypes { get; set; }
    }
}