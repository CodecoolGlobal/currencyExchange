using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Models
{
    public class Conversion
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public string BaseCurrency { get; set; }
        public string EndCurrency { get; set; }
        public decimal Amount { get; set; }
    }
}
