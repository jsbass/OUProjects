using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Calendar.Models;
using Calendar.Models.DB;

namespace Calendar.Helpers
{
    public static class CalendarMonthExtensions
    {
        private enum Sources
        {
            OuLiveWhale = 0,
            Athletics = 1
        }

        public static async Task UpdateDb()
        {
            var events = new List<CalendarEvent>();
            var ouEventsTask = GetOuEvents();

            await ouEventsTask;

            events.AddRange(ouEventsTask.Result);

            using (var db = DbHelper.GetDb())
            {
                foreach (var @event in events)
                {
                    if (
                        !db.CalendarEvents.Any(
                            e => e.fkGroupId == @event.fkGroupId && e.EventIdFromSource == @event.EventIdFromSource))
                    {
                        
                    }
                }
            }
        }
        
        public static async Task<List<CalendarEvent>> GetOuEvents()
        {
            const string baseUrl = "http://calendar.ou.edu/live/json/events";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var response = await client.GetAsync($@"{baseUrl}/exclude_group/Admin/exclude_group/livewhale"))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var events = JsonConvert.DeserializeObject<List<LiveWhaleEvent>>(content).Where(e => e.DateUtc.HasValue && e.Date2Utc.HasValue);

                    var results = new List<CalendarEvent>();
                    var tasks = new List<Task>();
                    foreach (var @event in events)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            var result = new CalendarEvent()
                            {
                                Contact = @event.ContactInfo,
                                ContactEmail = @event.CustomContactInfo,
                                Description = @event.Description,
                                StartTime = @event.DateUtc.Value,
                                EndTime = @event.Date2Utc.Value,
                                EventIdFromSource = @event.Id,
                                Location = @event.Location,
                                Url = @event.Url
                            };

                            result.fkGroupId =
                                (await DbHelper.GetGroup(@event.Gid, Sources.OuLiveWhale.GetHashCode())).GroupId;
                            results.Add(result);
                        }));
                    }

                    Task.WaitAll(tasks.ToArray());

                    return results;
                }
            }
        }

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