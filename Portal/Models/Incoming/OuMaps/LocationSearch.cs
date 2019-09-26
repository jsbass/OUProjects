using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Portal.Models.GeoJson;

namespace Portal.Models.OuMaps
{
    public class LocationSearch
    {
        [JsonProperty("data")]
        public List<SearchItem> Data { get; set; }
    }

    public class SearchItem
    {
        [JsonProperty("locurl")]
        public string Locurl { get; set; }

        [JsonProperty("locid")]
        public string Locid { get; set; }

        [JsonProperty("loctitle")]
        public string Loctitle { get; set; }

        [JsonProperty("displayAs")]
        public string DisplayAs { get; set; }

        [JsonProperty("geodata")]
        public Point Geodata { get; set; }

        [JsonProperty("polypaths")]
        public Polygon Polypaths { get; set; }
    }
}