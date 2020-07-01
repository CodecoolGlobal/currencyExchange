using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Models
{
    public class Conversion
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BaseCurrency { get; set; }
        public string EndCurrency { get; set; }
    }
}
