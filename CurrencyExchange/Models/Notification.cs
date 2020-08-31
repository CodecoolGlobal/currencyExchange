using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [EnumDataType(typeof(AboveOrUnder))]
        public AboveOrUnder AboveOrUnder { get; set; }

        public bool EmailSent { get; set; }

        [NotMapped]
        [Display(Name = "Actual Value")]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public decimal ActualValue { get; set; }
    }

    public enum AboveOrUnder
    {
            Above = 0,
            Under = 1
    }
}
