using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace RequirementsChecker.Models.Canvas
{
    [DataContract]
    public class Submission
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "grade")]
        public string Grade { get; set; }

        [DataMember(Name = "score")]
        public double Score { get; set; }

        [DataMember(Name = "submitted_at")]
        public DateTime? SubmittedAt { get; set; }

        [DataMember(Name = "assignment_id")]
        public int AssignmentId { get; set; }

        [DataMember(Name = "user_id")]
        public int UserId { get; set; }

        [DataMember(Name = "submission_type")]
        public string SubmissionType { get; set; }

        [DataMember(Name = "workflow_state")]
        public string WorkflowState { get; set; }

        [DataMember(Name = "grade_matches_current_submission")]
        public bool GradeMatchesCurrentSubmission { get; set; }

        [DataMember(Name = "graded_at")]
        public string GradedAt { get; set; }

        [DataMember(Name = "grader_id")]
        public int GraderId { get; set; }

        [DataMember(Name = "attempt")]
        public int? Attempt { get; set; }

        [DataMember(Name = "excused")]
        public bool Excused { get; set; }

        [DataMember(Name = "late_policy_status")]
        public string LatePolicyStatus { get; set; }

        [DataMember(Name = "points_deducted")]
        public double PointsDeducted { get; set; }

        [DataMember(Name = "late")]
        public bool Late { get; set; }

        [DataMember(Name = "missing")]
        public bool Missing { get; set; }

        [DataMember(Name = "seconds_late")]
        public double SecondsLate { get; set; }

        [DataMember(Name = "preview_url")]
        public string PreviewUrl { get; set; }

    }
}