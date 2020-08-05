﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Models
{
    public class Notification
    {
        public int ID { get; set; }
        public User User { get; set; }
        [Required]
        public string BaseCurrency { get; set; }

        [Required]
        public string EndCurrency { get; set; }
        
        [Required]
        public decimal Value { get; set; }

        [Required]
        public string AboverOrUnder { get; set; }
    }
}
