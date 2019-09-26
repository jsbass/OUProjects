using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Models
{
    public class Point : IGeometry
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public GeometryType Type => GeometryType.Point;

        [JsonProperty(PropertyName = "coordinates", Required = Required.Always)]
        public Position Position { get; set; }
    }
}