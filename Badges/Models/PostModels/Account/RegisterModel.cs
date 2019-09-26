using System.ComponentModel.DataAnnotations;

namespace Badges.Models.PostModels.Account
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "User ID")]
        [MaxLength(15)]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password must match")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(30)]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [MaxLength(30)]
        public string LastName { get; set; }

        [StringLength(9)]
        [Display(Name = "Sooner ID")]
        public string SoonerId { get; set; }
        [StringLength(8)]
        [Display(Name = "OU Net ID (4x4)")]
        public string OuNetId { get; set; }
    }
}