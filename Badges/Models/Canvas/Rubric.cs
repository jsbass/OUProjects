using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Badges.Models.Canvas
{
    [DataContract]
    public class Rubric
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "points")]
        public string Points { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "long_description")]
        public string LongDescription { get; set; }

        [DataMember(Name = "ratings")]
        public List<Rating> Ratings { get; set; }

        [DataMember(Name = "outcome_id")]
        public int? OutcomeId { get; set; }

        [DataMember(Name = "vendor_guid")]
        public object VendorGuid { get; set; }
    }
}