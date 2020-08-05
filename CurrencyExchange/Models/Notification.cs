using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.Models
{
    public class Notification
    {
        public int ID { get; set; }
        public virtual User User { get; set; }

        [Display(Name = "Base Currency")]
        [Required]
        public string BaseCurrency { get; set; }

        [Display(Name = "End Currency")]
        [Required]
        public string EndCurrency { get; set; }

        [Required]
        public decimal Value { get; set; }

        [Display(Name = "Above Or Under")]
        [Required]
        public string AboverOrUnder { get; set; }
    }
}
