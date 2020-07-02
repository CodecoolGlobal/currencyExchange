using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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


    }
}
