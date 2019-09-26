using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Portal.Models.GeoJson;

namespace Portal.Models.Outgoing.Map
{
    public class OverlayShapesContainer
    {
        [JsonProperty("locations")]
        public List<OverlayShape> Locations { get; set; }
    }

    public class OverlayShape
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("shape")]
        public Polygon Shape { get; set; }

        [JsonProperty("marker")]
        public Point Marker { get; set; }

        [JsonProperty("locationType")]
        public LocationTypes LocationType { get; set; }
    }
}