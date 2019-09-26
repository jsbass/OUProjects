using System.Runtime.Serialization;

namespace Badges.Models.Canvas
{
    [DataContract]
    public class ExternalToolTagAttributes
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "new_tab")]
        public bool? NewTab { get; set; }

        [DataMember(Name = "resource_link_id")]
        public string ResourceLinkId { get; set; }
    }
}