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

        public Email(string recipientEmail, string recipientUserName, string subject, string body)
        {
            RecipientEmail = recipientEmail;
            RecipientUserName = recipientUserName;
            Subject = subject ?? "";
            Body = body ?? "";
        }
    }
}
