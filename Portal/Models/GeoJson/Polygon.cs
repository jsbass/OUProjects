using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebGrease.Css;
using WebGrease.Css.Extensions;

namespace Portal.Models.GeoJson
{
    public class Polygon : IGeometry
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public GeometryType Type => GeometryType.Polygon;

        [JsonProperty("coordinates")]
        public List<List<Position>> Coordinates { get; set; }

        [JsonIgnore]
        public LineString OuterRing { get; set; }

        [JsonIgnore]
        public List<LineString> InnerRings { get; set; }

        public Polygon(List<List<Position>> coordinates)
        {
            Coordinates = coordinates;
            InnerRings = new List<LineString>();
            var lines = coordinates.Select(c => new LineString(c)).ToList();
            lines.ForEach((l, i) =>
            {
                //Added to fix OU's crappy api
                if (l.Positions.First() != l.Positions.Last())
                {
                    l.Positions.Add(l.Positions.First());
                }
                if(!isLinearRing(l)) throw new ArgumentException($"LineString {i+1} must be a linear ring");
                if (i == 0)
                {
                    //exterior is CCW
                    //if (getAreaSign(l) <= 0) throw new ArgumentException($"First line must be a counterclockwise wound linear ring: {JsonConvert.SerializeObject(l, Formatting.Indented)}");
                    OuterRing = l;
                    
                }
                if (i != 0)
                {
                    //interior is CW
                    //if (getAreaSign(l) > 0) throw new ArgumentException($"LineString {i+1} must be a clockwise wound linear ring: {JsonConvert.SerializeObject(l, Formatting.Indented)}");
                    InnerRings.Add(l);
                }
            });
        }

        private bool isLinearRing(LineString lineString)
        {
            var first = lineString.Positions.First();
            var last = lineString.Positions.Last();
            return lineString.Positions.Count >= 4 && first == last;
        }

        /// <summary>
        /// Gets the sign. A positive sign means CW, a negative sign means CCW, 0 means a no area (line)
        /// </summary>
        /// <param name="lineString"></param>
        /// <returns></returns>
        private int getAreaSign(LineString lineString)
        {
            double area = 0.0f;
            if(!isLinearRing(lineString)) throw new ArgumentException($"{nameof(lineString)} is not a closed polygon");
            //Ignore last point as it is a repeat of the first
            for (var i = 0; i < lineString.Positions.Count - 2; i++)
            {
                area += (lineString.Positions[i+1].Longitude - lineString.Positions[i].Longitude)*(lineString.Positions[i+1].Latitude + lineString.Positions[i].Latitude);
            }
            return Math.Sign(area);
        }
    }
}