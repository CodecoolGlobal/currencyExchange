using CurrencyExchange.Models;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MimeKit;
using System.Transactions;

namespace CurrencyExchange.Services
{
    public class MessageService
    {
        public static ImapClient client;
        private static string hostEmailAddress;
        private static string hostEmailPassword;
        private static readonly string Path = "./Resources/login.txt";

        public static void Initialize()
        {
            string[] lines = System.IO.File.ReadAllLines(Path);
            hostEmailAddress = lines[0];
            hostEmailPassword = lines[1];
            client = new ImapClient();
        }

        private static void SendMail(Email email)
        {
            MimeMessage message = new MimeMessage();
            BodyBuilder bodyBuilder = new BodyBuilder();
            message.From.Add(new MailboxAddress("Currency Exchange", hostEmailAddress));
            message.To.Add(new MailboxAddress(email.RecipientUserName, email.RecipientEmail));
            message.Subject = email.Subject;
            bodyBuilder.TextBody = email.Body;
            message.Body = bodyBuilder.ToMessageBody();
            SmtpClient client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.Connect("smtp.gmail.com", 587);
            client.Authenticate(hostEmailAddress, hostEmailPassword);
            client.Send(message);
            client.Disconnect(true);
        }

        public static void ComposeNotificationEmail(Notification notification)
        {
            string BaseCurrency = notification.BaseCurrency;
            string Endcurrency = notification.EndCurrency;
            string Value = notification.Value.ToString();
            string ActualValue = notification.ActualValue.ToString();

            string UserName = notification.User.UserName;
            string EmailAddress = notification.User.Email;

            string EmailBody = $"Dear {UserName}, \n" +
                $"\n" +
                $"The value of 1 {BaseCurrency} ha reached your target of {Value} {Endcurrency} as it is currently at {ActualValue} {Endcurrency}.\n" +
                $"\n" +
                $"Message automatically sent by CurrencyExchange (Made by Tamás Kruppa and Dávid Kalló)";
            Email email = new Email(EmailAddress, UserName, "Currency Notification", EmailBody);
            SendMail(email);
        }

        public static void ComposeTransactionEmail(Models.Transaction transaction)
        {
            string Currency = transaction.Currency;
            string Amount = transaction.Amount.ToString();
            string SenderUserName = transaction.Sender.UserName;
            string SenderEmailAddress = transaction.Sender.Email;
            string RecipientUserName = transaction.Sender.UserName;
            string RecipientEmailAddress = transaction.Recipient.Email;

            string SenderEmailBody = $"Dear {SenderUserName}, \n" +
                $"\n" +
                $"You have succesfully sent {Amount} {Currency} to {RecipientUserName}!" +
                $"\n" +
                $"Message automatically sent by CurrencyExchange (Made by Tamás Kruppa and Dávid Kalló)";

            Email senderEmail = new Email(SenderEmailAddress, SenderUserName, "Money Transfer", SenderEmailBody);
            SendMail(senderEmail);

            string RecipientEmailBody = $"Dear {RecipientUserName}, \n" +
               $"\n" +
               $"{SenderUserName} has sent you {Amount} {Currency}!" +
               $"\n" +
               $"Message automatically sent by CurrencyExchange (Made by Tamás Kruppa and Dávid Kalló)";

            Email recipientEmail = new Email(RecipientEmailAddress, RecipientUserName, "Money Transfer", RecipientEmailBody);
            SendMail(recipientEmail);
        }
    }
}
