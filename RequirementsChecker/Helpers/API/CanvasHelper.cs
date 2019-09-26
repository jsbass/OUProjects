using Newtonsoft.Json;
using RequirementsChecker.Models.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace RequirementsChecker.Helpers.API
{
    public static class CanvasHelper
    {

        private static string URL = @"https://canvas.ou.edu";
        private static string TOKEN = @"8808~DN4tn2So0LGQRy6tygZLIS8CrHJvJZejZTpplYquKx9O0Ln7zUQICNmLDdoaxElS";

        /// <summary>
        /// Gets the Submission for the Assignment for the Student
        /// </summary>
        /// <param name="ouNetId">Student's OUNetId.</param>
        /// <param name="assignmentId">Assignment's Id that you want the submission for.</param>
        /// <param name="courseId">The Course's Id that the Assignment is a part of.</param>
        /// <returns></returns>
        internal static async Task<Submission> GetSubmissionGradeOfAssignmentForUser(string ouNetId, string assignmentId, string courseId)
        {
            string url = $"{URL}/api/v1/courses/{courseId}/assignments/{assignmentId}/submissions/sis_login_id:{ouNetId}";
            Submission submission = await GetResultFromCanvas<Submission>(url);

            return submission;
        }

        /// <summary>
        /// Gets whether or not the student has completed the module or not.
        /// </summary>
        /// <param name="ouNetId">Student's OUNetId.</param>
        /// <param name="courseId">Course's Id that the Module is a part of.</param>
        /// <param name="moduleId">Module's Id that you want to check the completion status of.</param>
        /// <returns>
        /// True    : The Student has completed the Module.
        /// False   : The Student has not completed the Module. 
        /// </returns>
        internal static async Task<object> GetModuleStatusForUser(string ouNetId, string courseId, string moduleId)
        {
            string url = $"{URL}/api/v1/courses/{courseId}/modules/{moduleId}/modules?student_id=sis_login_id:{ouNetId}";
            User user = await GetResultFromCanvas<User>(url);

            return user;
        }

        /// <summary>
        /// Gets the Student's User object from Canvas.
        /// </summary>
        /// <param name="ouNetId">The Student's OUNetId</param>
        /// <returns>A Canvas User object belonging to the Student whose OUNetId you passed in.</returns>
        internal static async Task<User> GetCanvasUser(string ouNetId)
        {
            string url = $"{URL}/api/v1/users/sis_login_id:{ouNetId}/profile";
            User user = await GetResultFromCanvas<User>(url);

            return user;
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