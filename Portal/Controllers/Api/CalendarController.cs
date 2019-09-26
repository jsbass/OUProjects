using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Portal.Helpers;
using Portal.Models;

namespace Portal.Controllers.Api
{
    public class CalendarController : ApiController
    {
        [Route("api/calendar/ou")]
        public async Task<IHttpActionResult> GetOuEvents(int month = -1, int year = -1)
        {
            if (month == -1)
            {
                month = DateTime.Now.Month;
            }
            if (year == -1)
            {
                year = DateTime.Now.Year;
            }
            var monthObj = new Month(month, year);
            await monthObj.GetOuEvents();

            return Ok(monthObj);
        }
    }
}
