using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Models.GeoJson
{
    public class MultiPolygon : IGeometry
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public GeometryType Type => GeometryType.MultiPolygon;

        [JsonProperty("coordinates")]
        public List<List<List<Position>>> Coordinates { get; set; }

        public List<Polygon> Polygons { get; set; }

        public MultiPolygon(List<List<List<Position>>> coordinates)
        {
            Coordinates = coordinates;
            Polygons = coordinates.Select(c => new Polygon(c)).ToList();
        }
    }
}