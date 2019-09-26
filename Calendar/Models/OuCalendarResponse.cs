using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Calendar.Models
{
    public class OuCalendarResponse
    {

    }

    public class LiveWhaleEvent
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("gid")]
        public int Gid { get; set; }

        [JsonProperty("eid")]
        public string Eid { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("date_time")]
        public string DateTimeHtml { get; set; }

        [JsonProperty("date_ts")]
        public long DateTimestamp { get; set; }

        [JsonProperty("date_utc")]
        public DateTime? DateUtc { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("is_all_day")]
        public bool? IsAllDay { get; set; }

        [JsonProperty("repeats_until")]
        public string RepeatsUntil { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("location_title")]
        public string LocationTitle { get; set; }

        [JsonProperty("location_latitude")]
        public string LocationLatitude { get; set; }

        [JsonProperty("location_longitude")]
        public string LocationLongitude { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("tags_starred")]
        public string TagsStarred { get; set; }

        [JsonProperty("tags_global")]
        public string TagsGlobal { get; set; }

        [JsonProperty("contact_info")]
        public string ContactInfo { get; set; }

        [JsonProperty("custom_contact_info")]
        public string CustomContactInfo { get; set; }

        [JsonProperty("date2_utc")]
        public DateTime? Date2Utc { get; set; }
    }
}