using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Models.DB;

namespace Portal.Models.PostModels.Account
{
    public class EditModel
    {
        [EmailAddress]
        public string Email { get; set; }

        [HiddenInput]
        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password must match")]
        [DataType(DataType.Password)]
        public string ConfirmPwd { get; set; }

        [Display(Name = "Roles")]
        public List<int> RoleIds { get; set; }
    }
}