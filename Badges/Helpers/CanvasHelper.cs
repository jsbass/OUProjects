using Badges.Models.Canvas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Badges.Helpers
{
    public static class CanvasHelper
    {

        private static string URL = @"https://canvas.ou.edu";
        private static string TOKEN = @"8808~DN4tn2So0LGQRy6tygZLIS8CrHJvJZejZTpplYquKx9O0Ln7zUQICNmLDdoaxElS";

        /// <summary>
        /// Gets the modules for a given course.
        /// </summary>
        /// <param name="courseId">The Course's Id that you want the modules from.</param>
        /// <returns>A list of Modules that belong to the course.</returns>
        internal static async Task<List<Module>> GetModulesForCourse(string courseId)
        {
            string url = $"{URL}/api/v1/courses/{courseId}/modules";
            var modules = await GetResultFromCanvas<List<Module>>(url);

            return modules;
        }

        /// <summary>
        /// Gets the assignments for a given course.
        /// </summary>
        /// <param name="courseId">The Course's Id that you want the assignments from.</param>
        /// <returns>A list of Assignments that belong to the course.</returns>
        internal static async Task<List<Assignment>> GetAssignmentsForCourse(string courseId)
        {
            string url = $"{URL}/api/v1/courses/{courseId}/assignments";
            var assignments = await GetResultFromCanvas<List<Assignment>>(url);

            return assignments;
        }


        /// <summary>
        /// Private function that takes the URL to hit Canvas with and return the result as an object of type T
        /// </summary>
        /// <typeparam name="T">Class of what the Canvas API at the passed in URL will return</typeparam>
        /// <param name="url">URL for the Canvas API to hit</param>
        /// <returns>The object returned from the Canvas API from the passed in URL</returns>
        private static async Task<T> GetResultFromCanvas<T>(string url)
        {
            T result = default(T);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                var response = await (await client.GetAsync(url)).Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<T>(response);

                if (result == null)
                {
                    throw new Exception("Deserialized JSON is null");
                }
            }

            return result;
        }
    }
}