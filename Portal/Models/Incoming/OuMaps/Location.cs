using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Portal.Models.Incoming.OuMaps
{
    public class LocationContainer<T> where T : Location
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
    public class Location
    {
        [JsonProperty("loctitle")]
        public string Loctitle { get; set; }

        [JsonProperty("loccode")]
        public string Loccode { get; set; }

        [JsonProperty("loctext")]
        public string Loctext { get; set; }

        [JsonProperty("lockeys")]
        public string Lockeys { get; set; }

        [JsonProperty("locimg")]
        public string Locimg { get; set; }

        [JsonProperty("locid")]
        public string Locid { get; set; }

        [JsonProperty("locurl")]
        public string Locurl { get; set; }

        [JsonProperty("locoptions")]
        public List<string> Locoptions { get; set; }

        [JsonProperty("geodata")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string Geodata { get; set; }
    }

    public class Parking : Location
    {
        [JsonProperty("parking")]
        public bool isParking { get; set; }

        [JsonProperty("handicappedcount")]
        public string Handicappedcount { get; set; }

        [JsonProperty("spacecount")]
        public string Spacecount { get; set; }

        [JsonProperty("parkingtypes")]
        public List<string> Parkingtypes { get; set; }
    }

    public class Building : Location
    {
        [JsonProperty("accessibility")]
        public string Accessibility { get; set; }

        [JsonProperty("floorplan")]
        public string Floorplan { get; set; }
    }

    public class JsonStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(JTokenType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object)
            {
                return token.ToString();
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var token = JToken.Parse(value.ToString());
            writer.WriteToken(token.CreateReader());
        }
    }
}