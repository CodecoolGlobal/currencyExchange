using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyExchange.Models
{
    public class User
    {
        public int ID { get; set; }

        [Display(Name = "User Name")]
        [StringLength(20, MinimumLength = 3)]
        [Required]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress), Display(Name = "Email Address")]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8)]
        [Required]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password), Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public Role Role { get; set; }
    }

    public enum Role
    {
        User = 0,
        Admin = 1
    }
}
