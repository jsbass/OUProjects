using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Calendar.Models
{
    public class Event
    {
        public DateTime DateTime { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("contactInfo")]
        public string ContactInfo { get; set; }

        [JsonProperty("contactEmail")]
        public string ContactEmail { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("gid")]
        public int Gid { get; set; }
    }

    public class Month
    {
        [JsonProperty("days")]
        public List<Event>[] Days { get; set; }

        [JsonProperty("numMonth")]
        public int NumMonth { get; set; }

        [JsonProperty("year")]
        public int Year { get; private set; }

        [JsonProperty("numberOfDays")]
        public int NumberOfDays => DateTime.DaysInMonth(Year, NumMonth);

        [JsonProperty("startDayOfWeek")]
        public int StartDayOfWeek => new DateTime(Year, NumMonth, 1).DayOfWeek.GetHashCode();

        public Month(MonthOfYear month)
        {
            Year = DateTime.Now.Year;
            NumMonth = month.GetHashCode();
            Days = new List<Event>[NumberOfDays + 1];
            for (int i = 1; i <= NumberOfDays; i++)
            {
                Days[i] = new List<Event>();
            }
        }

        public Month(int month)
        {
            Year = DateTime.Now.Year;
            NumMonth = month;
            Days = new List<Event>[NumberOfDays + 1];
            for (int i = 1; i <= NumberOfDays; i++)
            {
                Days[i] = new List<Event>();
            }
        }

        public Month(MonthOfYear month, int year)
        {
            Year = year;
            NumMonth = month.GetHashCode();
            Days = new List<Event>[NumberOfDays + 1];
            for (int i = 1; i <= NumberOfDays; i++)
            {
                Days[i] = new List<Event>();
            }
        }

        public Month(int month, int year)
        {
            Year = year;
            NumMonth = month;
            Days = new List<Event>[NumberOfDays + 1];
            for (int i = 1; i <= NumberOfDays; i++)
            {
                Days[i] = new List<Event>();
            }
        }

        public static Month MergeEvents(Month first, Month second)
        {
            for (var i = 1; i < second.NumberOfDays; i++)
            {
                try
                {
                    first.Days[i].AddRange(second.Days[i]);
                }
                catch (Exception)
                {
                    //ignore
                }
                
            }

            return first;
        }

        public Month MergeEvents(Month other)
        {
            try
            {
                for (var i = 1; i < other.NumberOfDays; i++)
                {
                    Days[i].AddRange(other.Days[i]);
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return this;
        }
    }

    public enum MonthOfYear
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
}