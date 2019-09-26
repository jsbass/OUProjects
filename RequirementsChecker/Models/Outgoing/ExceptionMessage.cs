using System.Runtime.Serialization;

namespace RequirementsChecker.Models.Outgoing
{
    [DataContract]
    public class ExceptionMessage
    {
        [DataMember(Name = "message")]
        public string Message { get; set; }

    }
}