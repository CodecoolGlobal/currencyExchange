using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace CurrencyExchange.Models
{
    public class Transaction
    {
        public int ID { get; set; }
        public User Sender { get; set; }
        public User Recipient { get; set; }
        public string Currency { get; set; }
        public int Amount { get; set; }
    }
}
