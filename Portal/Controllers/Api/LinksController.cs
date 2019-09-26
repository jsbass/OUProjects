using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Portal.Helpers;
using Portal.Models;
using Portal.Models.DB;
using Newtonsoft.Json;

namespace Portal.Controllers.Api
{
    public class LinksController : ApiController
    {
        [Route("api/links")]
        public async Task<IHttpActionResult> GetLinks()
        {
            List<Category> links = new List<Category>();
            using (var db = DbHelper.GetDb())
            {
                links = await db.GetCategories(null);                
            }
            List<Models.Json.Links.Category> categories = new List<Models.Json.Links.Category>();
            foreach (var cat in links)
            {
                categories.Add(new Models.Json.Links.Category(cat));
            }


            return Ok(categories);
        }
    }
}
