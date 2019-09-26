using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;

namespace Portal.Models
{
    public class LogInModel
    {
        [Required]
        [DisplayName("OU Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        //Use to give password restrictions
        //[RegularExpression(@"","Password must ...")]
        public string Password { get; set; }

        [Required]
        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }
}