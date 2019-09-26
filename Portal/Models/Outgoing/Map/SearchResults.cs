using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Portal.Models.Outgoing.Map
{
    public class SearchResults
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("scores")]
        public List<SearchResult> Results { get; set; }

        public SearchResults() : this(new List<SearchResult>()) { }

        public SearchResults(List<SearchResult> shapes)
        {
            Meta = new Meta();
            Results = shapes;
            Meta.Count = Results.Count;
        }
    }

    public class Meta
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }

    public class ScoreContainer
    {
        public int Score { get; set; }
        public SearchResult Result { get; set; }
    }

    public class SearchResult : OverlayShape
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}