using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Portal.Models.DB;
using Portal.Models.Incoming.OuMaps;
using Portal.Models.OuMaps;
using Location = Portal.Models.Incoming.OuMaps.Location;

namespace Portal.Helpers
{
    public class OuMapHelper
    {
        public async Task<List<SearchItem>> GetBuildings()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync("https://ou.edu/home/normanmap/mapgeo.json/o:building");
                var content = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<LocationSearch>(content);
                return obj.Data;
            }
        }

        public async Task<List<SearchItem>> GetParking()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync("https://ou.edu/home/normanmap/mapgeo.json/o:parking");
                var content = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<LocationSearch>(content);
                return obj.Data;
            }
        }

        public async Task<List<SearchItem>> GetLocations()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync("https://ou.edu/home/normanmap/mapgeo.json/o:location");
                var content = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<LocationSearch>(content);
                return obj.Data;
            }
        }

        public async Task<T> GetLocation<T>(string locUrl) where T : Location
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync($"https://ou.edu/{locUrl.TrimStart('/')}/_jcr_content.json");
                var content = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<LocationContainer<T>>(content);
                return obj.Data;
            }
        }
    }
}