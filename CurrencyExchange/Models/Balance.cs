using System;
using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.Models
{
    public class Balance
    {
        public int ID { get; set; }
        public virtual User User { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int Amount { get; set; }
    }
}
