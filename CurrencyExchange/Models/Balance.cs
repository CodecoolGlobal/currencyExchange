using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace CurrencyExchange.Models
{
    public class Balance
    {
        public int ID { get; set; }
        public User User { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public int Amount { get; set; }
    }
}
