using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Badges.Models.PostModels.Account
{
    public class LogInModel
    {
        [Required]
        [DisplayName("UserId")]
        public string UserId { get; set; }

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