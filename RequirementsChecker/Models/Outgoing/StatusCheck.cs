using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace RequirementsChecker.Models.Outgoing
{
    [DataContract]
    public class StatusCheck
    {
        [DataMember(Name = "isComplete")]
        public bool IsComplete { get; set; }

        [DataMember(Name = "percentComplete")]
        public double PercentComplete { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "lastUpdated")]
        public DateTime? LastUpdated { get; set; }

        [DataMember(Name = "exception")]
        public ExceptionMessage Exception { get; set; }

        public StatusCheck()
        {
            this.IsComplete = false;
            this.PercentComplete = 0.0;
            this.Status = "";
            this.LastUpdated = null;
            this.Exception = null;
        }
    }
}