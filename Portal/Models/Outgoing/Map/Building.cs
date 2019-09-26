using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Models.GeoJson;

namespace Portal.Models.Outgoing.Map
{
    public class BuildingContainer
    {
        [JsonProperty("buildings")]
        public List<Building> Buildings { get; set; }
    }

    public class Building : Location
    {
        public Building()
        {
            LocationType = LocationTypes.Building;
        }
    }
}