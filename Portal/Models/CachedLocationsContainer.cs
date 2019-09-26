using System;
using System.Collections.Generic;
using System.Linq;
using Portal.Helpers;
using Portal.Models.Outgoing.Map;

namespace Portal.Models
{
    public class CachedLocationsContainer
    {
        public CachedLocationsContainer()
        {
            Buildings = new List<Building>();
            Parkings = new List<Parking>();
        }

        public RectangleD Bounds = new RectangleD(0,0,0,0);

        public List<Building> Buildings { get; set; }
        public List<Parking> Parkings { get; set; }

        public List<Location> Locations
        {
            get
            {
                var list = new List<Location>();
                list.AddRange(Buildings);
                list.AddRange(Parkings);
                return list;
            }
        }

        public void UpdateBounds()
        {
            var minX = double.MaxValue;
            var minY = double.MaxValue;

            var maxX = double.MinValue;
            var maxY = double.MinValue;

            foreach (var loc in Locations)
            {
                minX = minX < loc.BoundingBox.Left ? minX : loc.BoundingBox.Left;
                minY = minY < loc.BoundingBox.Bottom ? minY : loc.BoundingBox.Bottom;

                maxX = maxX > loc.BoundingBox.Right ? maxX : loc.BoundingBox.Right;
                maxY = maxY > loc.BoundingBox.Top ? maxY : loc.BoundingBox.Top;
            }

            Bounds = RectangleD.FromLTRB(minX, maxY, maxX, minY);
        }

        public SearchResults Search(string search)
        {
            var scores = GetScores(search).Where(s => s.Score > 0).OrderByDescending(s => s.Score);
            return new SearchResults(scores.Select(s => s.Result).ToList());
        }

        public SearchResults Search(string search, int take, int skip = 0)
        {
            var scores = GetScores(search).OrderByDescending(s => s.Score).Skip(skip).Take(take);
            return new SearchResults(scores.Select(s => s.Result).ToList());
        }

        private IEnumerable<ScoreContainer> GetScores(string search)
        {
            var tokens = search.Split(',');

            return Locations.Select(l => new ScoreContainer()
            {
                Score = tokens.Sum(t =>
                    (l.Description.ToLower().Contains(t.ToLower()) ? 10 : 0) +
                    (l.Name.ToLower().Contains(t.ToLower()) ? 20 : 0) +
                    (l.OuCode.ToLower().Contains(t.ToLower()) ? 30 : 0)),
                Result = new SearchResult()
                {
                    Id = l.Id ?? 0,
                    LocationType = l.LocationType,
                    Marker = l.Marker,
                    Shape = l.Shape,
                    Title = l.Name
                }
            });
        }
    }
}