using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Models
{
    public class MultiPoint : IGeometry
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public GeometryType Type => GeometryType.MultiPoint;

        [JsonProperty(PropertyName = "coordinates", Required = Required.Always)]
        public List<Position> Positions { get; set; }
    }
}