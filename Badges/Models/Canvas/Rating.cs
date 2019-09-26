using System.Runtime.Serialization;

namespace Badges.Models.Canvas
{
    [DataContract]
    public class Rating
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "points")]
        public string Points { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}