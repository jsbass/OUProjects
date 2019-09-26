using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Portal.Models;

namespace Portal.Helpers
{
    public static class CalendarMonthExtensions
    {
        public static async Task<Month> GetOuEvents(this Month month)
        {
            const string baseUrl = "http://calendar.ou.edu/live/json/events";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var response = await client.GetAsync($@"{baseUrl}/exclude_group/Admin/exclude_group/livewhale/start_date/01{month.NumMonth}{month.Year}/end_date/{month.NumberOfDays}{month.NumMonth}{month.Year}"))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var events = JsonConvert.DeserializeObject<List<LiveWhaleEvent>>(content);

                    foreach (var evt in events)
                    {
                        if (evt.DateUtc.HasValue)
                        {
                            var day = evt.DateUtc.Value.ToLocalTime().Day;
                            month.Days[day].Add(new Event()
                            {
                                ContactEmail = evt.CustomContactInfo,
                                ContactInfo = evt.ContactInfo,
                                DateTime = evt.DateUtc.Value,
                                Description = evt.Description,
                                Location = evt.Location,
                                Thumbnail = evt.Thumbnail,
                                Title = evt.Title,
                                Group = evt.Group,
                                Gid = evt.Gid
                            });
                        }
                    }
                }
            }

            return month;
        }
    }
}