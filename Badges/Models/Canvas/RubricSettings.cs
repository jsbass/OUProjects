using System.Runtime.Serialization;

namespace Badges.Models.Canvas
{
    [DataContract]
    public class RubricSettings
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "points_possible")]
        public string PointsPossible { get; set; }

        [DataMember(Name = "free_form_criterion_comments")]
        public bool FreeFormCriterionComments { get; set; }
    }
}