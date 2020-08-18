using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace CurrencyExchange.Models
{
    public class Transaction
    {
        public int ID { get; set; }
        public User Sender { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public User Recipient { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int Amount { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; }
    }
}
