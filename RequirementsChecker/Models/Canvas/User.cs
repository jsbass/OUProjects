using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace RequirementsChecker.Models.Canvas
{
    [DataContract]
    public class User
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "sortable_name")]
        public string SortableName { get; set; }

        [DataMember(Name = "short_name")]
        public string ShortName { get; set; }

        [DataMember(Name = "sis_user_id")]
        public string SisUserId { get; set; }

        [DataMember(Name = "integration_id")]
        public string IntegrationId { get; set; }

        [DataMember(Name = "sis_login_id")]
        public string SisLoginId { get; set; }

        [DataMember(Name = "sis_import_id")]
        public int? SisImportId { get; set; }

        [DataMember(Name = "login_id")]
        public string LoginId { get; set; }

        [DataMember(Name = "avatar_url")]
        public string AvatarUrl { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "bio")]
        public string Bio { get; set; }

        [DataMember(Name = "primary_email")]
        public string PrimaryEmail { get; set; }

        [DataMember(Name = "time_zone")]
        public string TimeZone { get; set; }

        [DataMember(Name = "locale")]
        public object Locale { get; set; }
    }
}