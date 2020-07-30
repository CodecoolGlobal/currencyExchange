using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Models
{
    public class Email
    {
        public string From { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientUserName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Date { get; set; }

        public Email(string recipient, string subject, string body)
        {
            RecipientEmail = recipient;
            Subject = subject ?? "";
            Body = body ?? "";
        }
    }
}
