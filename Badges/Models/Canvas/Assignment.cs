using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Badges.Models.Canvas
{
    [DataContract]
    public class Assignment
    {
        [DataMember(Name = "id")]
        public int? Id { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "due_at")]
        public DateTime? DueAt { get; set; }

        [DataMember(Name = "unlock_at")]
        public DateTime? UnlockAt { get; set; }

        [DataMember(Name = "lock_at")]
        public DateTime? LockAt { get; set; }

        [DataMember(Name = "points_possible")]
        public string PointsPossible { get; set; }

        [DataMember(Name = "grading_type")]
        public string GradingType { get; set; }

        [DataMember(Name = "assignment_group_id")]
        public int? AssignmentGroupId { get; set; }

        [DataMember(Name = "grading_standard_id")]
        public int? GradingStandardId { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "updated_at")]
        public string UpdatedAt { get; set; }

        [DataMember(Name = "peer_reviews")]
        public bool? PeerReviews { get; set; }

        [DataMember(Name = "automatic_peer_reviews")]
        public bool? AutomaticPeerReviews { get; set; }

        [DataMember(Name = "position")]
        public int? Position { get; set; }

        [DataMember(Name = "grade_group_students_individually")]
        public bool? GradeGroupStudentsIndividually { get; set; }

        [DataMember(Name = "anonymous_peer_reviews")]
        public bool? AnonymousPeerReviews { get; set; }

        [DataMember(Name = "group_category_id")]
        public int? GroupCategoryId { get; set; }

        [DataMember(Name = "post_to_sis")]
        public bool? PostToSis { get; set; }

        [DataMember(Name = "moderated_grading")]
        public bool? ModeratedGrading { get; set; }

        [DataMember(Name = "omit_from_final_grade")]
        public bool? OmitFromFinalGrade { get; set; }

        [DataMember(Name = "course_id")]
        public int? CourseId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "submission_types")]
        public List<string> SubmissionTypes { get; set; }

        [DataMember(Name = "has_submitted_submissions")]
        public bool? HasSubmittedSubmissions { get; set; }

        [DataMember(Name = "muted")]
        public bool? Muted { get; set; }

        [DataMember(Name = "html_url")]
        public string HtmlUrl { get; set; }

        [DataMember(Name = "has_overrides")]
        public bool? HasOverrides { get; set; }

        [DataMember(Name = "needs_grading_count")]
        public int? NeedsGradingCount { get; set; }

        [DataMember(Name = "integration_id")]
        public string IntegrationId { get; set; }

        [DataMember(Name = "published")]
        public bool? Published { get; set; }

        [DataMember(Name = "unpublishable")]
        public bool? Unpublishable { get; set; }

        [DataMember(Name = "only_visible_to_overrides")]
        public bool? OnlyVisibleToOverrides { get; set; }

        [DataMember(Name = "locked_for_user")]
        public bool? LockedForUser { get; set; }

        [DataMember(Name = "submissions_download_url")]
        public string SubmissionsDownloadUrl { get; set; }

        [DataMember(Name = "external_tool_tag_attributes")]
        public ExternalToolTagAttributes ExternalToolTagAttributes { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "use_rubric_for_grading")]
        public bool? UseRubricForGrading { get; set; }

        [DataMember(Name = "free_form_criterion_comments")]
        public bool? FreeFormCriterionComments { get; set; }

        [DataMember(Name = "rubric")]
        public List<Rubric> Rubric { get; set; }

        [DataMember(Name = "rubric_settings")]
        public RubricSettings RubricSettings { get; set; }
    }

}