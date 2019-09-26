using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class SendEmailConfirmModel
    {
        [Required]
        [DisplayName("OU Email")]
        [EmailAddress]
        public string Email { get; set; }
    }
}