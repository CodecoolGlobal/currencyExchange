using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Models
{
    public class User
    {
        public int ID { get; set; }

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

        public string Role { get; set; }
    }
}
