using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Badges.Models
{
    public class ApiAnswer
    {
        [JsonProperty("isComplete")]
        public bool IsComplete { get; set; }

        [JsonProperty("percentComplete")]
        public float PercentComplete { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}