using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Portal.Helpers;
using Portal.Models.GeoJson;

namespace Portal.Models.Outgoing.Map
{
    public class Location : IRegion
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("ouCode")]
        public string OuCode { get; set; }

        [JsonProperty("locationType")]
        public LocationTypes LocationType { get; protected set; }

        [JsonProperty("searchTokensString")]
        public string SearchTokensString => $"{OuCode} {Name} {string.Join(" ", Tags)}";

        [JsonProperty("shape")]
        public Polygon Shape { get; set; }
        
        [JsonProperty("marker")]
        public Point Marker { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("imgUrl")]
        public string ImgUrl { get; set; }

        [JsonProperty("bbox")]
        public RectangleD BoundingBox
        {
            get
            {
                if (_bBox == null)
                {
                    _bBox = CalcBBox();
                }

                return _bBox;
            }
        }

        private RectangleD _bBox;

        public Location()
        {
            LocationType = LocationTypes.None;
            Tags = new List<string>();
        }

        public RectangleD CalcBBox()
        {
            var minX = double.MaxValue;
            var minY = double.MaxValue;

            var maxX = double.MinValue;
            var maxY = double.MinValue;

            foreach (var coordList in Shape.Coordinates)
            {
                foreach (var coord in coordList)
                {
                    minX = Math.Min(coord.Longitude, minX);
                    maxX = Math.Max(coord.Longitude, maxX);

                    minY = Math.Min(coord.Latitude, minY);
                    maxY = Math.Max(coord.Latitude, maxY);
                }
            }

            return RectangleD.FromLTRB(minX, maxY, maxX, minY);
        }

        public bool Contains(PointD p)
        {
            if (!BoundingBox.Contains(p)) return false;
            
            //avg of height and width divided by 100
            var e = (BoundingBox.Width + BoundingBox.Height) / 200;
            var ray = new LineD(BoundingBox.X - e, BoundingBox.Y + e, p.X, p.Y);

            var intersections = 0;
            LineD line;
            for (var i = 0; i < Shape.OuterRing.Positions.Count - 1; i++)
            {
                line = new LineD(Shape.OuterRing.Positions[i].Longitude, Shape.OuterRing.Positions[i].Latitude, Shape.OuterRing.Positions[i+1].Longitude, Shape.OuterRing.Positions[i+1].Latitude);
                if (line.Intersects(ray)) intersections++;
            }
            foreach (var lineString in Shape.InnerRings)
            {
                for (var i = 0; i < Shape.OuterRing.Positions.Count - 1; i++)
                {
                    line = new LineD(Shape.OuterRing.Positions[i].Longitude, Shape.OuterRing.Positions[i].Latitude, Shape.OuterRing.Positions[i + 1].Longitude, Shape.OuterRing.Positions[i + 1].Latitude);
                    if (line.Intersects(ray)) intersections++;
                }
            }

            return intersections % 2 == 1;
        }
        
        public bool IsContainedBy(RectangleD r)
        {
            return r.Contains(BoundingBox);
        }

        public bool Overlaps(RectangleD r)
        {
            if (!r.IntersectsWith(BoundingBox)) return false;

            foreach (var coordList in Shape.Coordinates)
            {
                foreach (var coord in coordList)
                {
                    if (r.Contains(coord.Longitude, coord.Latitude)) return true;
                }
            }

            return false;
        }
    }

    public enum LocationTypes
    {
        None = 0,
        Building = 1,
        Parking = 2
    }
}