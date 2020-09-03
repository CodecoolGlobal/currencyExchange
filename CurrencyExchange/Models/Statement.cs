using System;

namespace CurrencyExchange.Models
{
    public class Statement
    {
        public int Id { get; set; }
        public User User { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FilePath { get; set; }
    }
}
