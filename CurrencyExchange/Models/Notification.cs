using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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

        [NotMapped]
        [Display(Name = "Actual Value")]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public decimal ActualValue { get; set; }
    }
}
