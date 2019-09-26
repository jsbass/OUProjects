using Newtonsoft.Json;
using Portal.Models.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Models.Incoming.OuMaps
{
    public class OuMapsCustomGeoData
    {
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("locality")]
        public string locality { get; set; }
        [JsonProperty("postalcode")]
        public string PostalCode { get; set; }
        [JsonProperty("region")]
        public string region { get; set; }
        [JsonProperty("street_address")]
        public string StreetAddress { get; set; }
        [JsonProperty("Polygon")]
        public Polygon Polygon { get; set; }
        [JsonProperty("bnd_e")]
        public double BndE { get; set; }
        [JsonProperty("bnd_w")]
        public double BndW { get; set; }
        [JsonProperty("bnd_n")]
        public double BndN { get; set; }
        [JsonProperty("bnd_s")]
        public double BndS { get; set; }
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }
}