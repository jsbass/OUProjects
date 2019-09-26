using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Badges.Models.PostModels.Account
{
    public class SendEmailConfirmModel
    {
        [Required]
        [DisplayName("OU Email")]
        [EmailAddress]
        public string Email { get; set; }
    }
}