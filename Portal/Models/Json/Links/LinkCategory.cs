using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Models.Json.Links
{
    public class LinkCategory
    {

        [JsonProperty("linkId")]
        public int LinkId { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }

        public LinkCategory(DB.LinkCategory lc)
        {
            this.LinkId = lc.LinkId;
            this.Rank = lc.Rank;
        }
    }
}