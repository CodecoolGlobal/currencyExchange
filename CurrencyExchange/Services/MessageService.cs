using CurrencyExchange.Models;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Services
{
    public class MessageService
    {
        public static ImapClient client;
        private static string hostEmailAddress;
        private static string hostEmailPassword;
        readonly string Path = "./Resources/login.txt";
       
        public MessageService()
        {
            string[] lines = System.IO.File.ReadAllLines(Path);
            hostEmailAddress = lines[0];
            hostEmailPassword = lines[1];

            client = new ImapClient();
            Connection(hostEmailAddress, hostEmailPassword);
        }

        public static void Connection(string userName, string password)
        {
            client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
            client.Authenticate(userName, password);
        }

        public static void SendMail(Email email)
        {
            MimeMessage message = new MimeMessage();
            BodyBuilder bodyBuilder = new BodyBuilder();
            message.From.Add(new MailboxAddress("Currency Exchange", hostEmailAddress));
            message.To.Add(new MailboxAddress(email.RecipientUserName, email.RecipientEmail));
            message.Subject = email.Subject;
            bodyBuilder.HtmlBody = email.Body;
            message.Body = bodyBuilder.ToMessageBody();
            SmtpClient client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.Connect("smtp.gmail.com", 587);
            client.Authenticate(hostEmailAddress, hostEmailPassword);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
