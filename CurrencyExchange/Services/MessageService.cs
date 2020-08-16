using CurrencyExchange.Models;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MimeKit;

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

        public static void SendMail(Email email)
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

            string emailBody = $"Dear {UserName}, \n" +
                $"\n" +
                $"The value of 1 {BaseCurrency} ha reached your target of {Value} {Endcurrency} as it is currently at {ActualValue} {Endcurrency}.\n" +
                $"\n" +
                $"Message automatically sent by CurrencyExchange (Made by Tamás Kruppa and Dávid Kalló)";
            Email email = new Email(EmailAddress, UserName, "Currency Notification", emailBody);
            SendMail(email);
        }
    }
}
