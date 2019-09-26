using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Badges.Models.Canvas
{
    [DataContract]
    public class Module
    {
        [DataMember(Name = "id")]
        public int id { get; set; }


        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "position")]
        public int Position { get; set; }


        [DataMember(Name = "unlock_at")]
        public string UnlockAt { get; set; }


        [DataMember(Name = "require_sequential_progress")]
        public bool RequireSequentialProgress { get; set; }


        [DataMember(Name = "publish_final_grade")]
        public bool PublishFinalGrade { get; set; }


        //[DataMember(Name = "prerequisite_module_ids")]
        //public List<int> prerequisite_module_ids { get; set; }


        [DataMember(Name = "published")]
        public bool Published { get; set; }


        [DataMember(Name = "items_count")]
        public int ItemsCount { get; set; }


        [DataMember(Name = "items_url")]
        public string ItemsUrl { get; set; }
    }

}