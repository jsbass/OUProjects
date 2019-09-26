using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Portal.Models
{
    public class MultiLineString : IGeometry
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public GeometryType Type => GeometryType.MultiLineString;

        [JsonProperty("coordinates")]
        public List<List<Position>> Coordinates { get; set; }

        [JsonIgnore]
        public List<LineString> Lines { get; set; }

        public MultiLineString(List<List<Position>> coordinates)
        {
            Coordinates = coordinates;
            foreach (var coordinate in coordinates)
            {
                Lines.Add(new LineString(coordinate));
            }
        }
    }
}