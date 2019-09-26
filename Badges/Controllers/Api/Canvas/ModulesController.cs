using Badges.Helpers;
using Badges.Models.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Badges.Controllers.Api.Canvas
{
    public class ModulesController : ApiController
    {
        public async Task<List<Module>> Get(string courseId)
        {
            var modules = await CanvasHelper.GetModulesForCourse(courseId);

            return modules;
        }
    }
}
