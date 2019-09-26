using RequirementsChecker.Helpers;
using RequirementsChecker.Helpers.API;
using RequirementsChecker.Models.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RequirementsChecker.Controllers.Api
{
    /// <summary>
    /// API for checking assignments in Canvas
    /// </summary>
    public class AssignmentController : ApiController
    {
        /// <summary>
        /// Checks to see whether the student's submission for the course and assignment meets the minimum grade.
        /// </summary>
        /// <param name="ouNetId">The students OUNetId.</param>
        /// <param name="assignmentId">The Id of the Assignment the submission is for.</param>
        /// <param name="courseId">The Id of the Course that the Assignment is part of.</param>
        /// <param name="minimumGrade">The minimum grade for the Student to have to be considered complete.</param>
        /// <returns>A StatusCheck object with information about whether the Student has met the requirements.</returns>
        [ResponseType(typeof(StatusCheck))]
        [Route("canvas/student/{ouNetId}/course/{courseId}/assignment/{assignmentId}/v1")]
        public async Task<StatusCheck> Get(string ouNetId, string assignmentId, string courseId, string minimumGrade)
        {
            var result = new StatusCheck();
            var assignmentSubmission = await CanvasHelper.GetSubmissionGradeOfAssignmentForUser(ouNetId, assignmentId, courseId);

            if (minimumGrade.IsNumber())
            {
                if (assignmentSubmission.Grade.IsNumber())
                {
                    if (Convert.ToDouble(assignmentSubmission.Grade) >= Convert.ToDouble(minimumGrade))
                    {
                        result.IsComplete = true;
                        result.PercentComplete = 1.0;
                    }
                }
                else
                {
                    result.IsComplete = false;
                    result.Exception = new ExceptionMessage() { Message = @"Grade passed in to check was a number, but the grade value for the assignment is not numeric." };
                }
            }
            else if (minimumGrade.IsAlphaGrade())
            {
                if (minimumGrade.CompareAlphaGrades(assignmentSubmission.Grade) >= 0)
                {
                    result.IsComplete = true;
                    result.PercentComplete = 1.0;
                }
                else
                {
                    result.IsComplete = false;
                }
            }
            else if (assignmentSubmission.Grade.IsPassed())
            {
                result.IsComplete = true;
                result.PercentComplete = 1.0;
            }

            return result;
        }

    }
}
