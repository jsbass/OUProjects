using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Models
{
    public class LineString : IGeometry
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public GeometryType Type => GeometryType.LineString;

        [JsonProperty("coordinates")]
        public List<Position> Positions { get; set; }

        [JsonConstructor]
        public LineString(List<Position> coordinates)
        {
            if (coordinates.Count < 2)
            {
                throw new ArgumentException("There must be at least 2 coordinates");
            }
            Positions = coordinates;
        }
    }
}