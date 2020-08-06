using CurrencyExchange.Controllers;
using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

namespace CurrencyExchange.Services
{
    public class NotificationService
    {
        private static IServiceProvider _serviceProvider;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Timer timer = new Timer();
            timer.AutoReset = true;

            //interval is in miliseconds 
            //1000 units = 1 second
            //10000 = 10 seconds
            //60000 = 1 minute
            //3600000 = 1 hour
            timer.Interval = 10000;

            timer.Elapsed += CheckNotifications;

            bool TestEmailSending = true;

            timer.Enabled = false;

            if (TestEmailSending)
            {
                timer.Enabled = true;
                timer.Start();
            }

        }

        private async static void CheckNotifications(object sender, EventArgs e)
        {
            List<Notification> notifications = await GetNotificationsAsync(null);
            List<Notification> notificationsToSend = new List<Notification>();

            //check which notifications meet the criteria given by the users
            foreach (Notification notification in notifications)
            {
                if (notification.EmailSent == false &&
                    notification.AboveOrUnder.Equals("above") &&
                    notification.ActualValue >= notification.Value)
                {
                    notificationsToSend.Add(notification);
                }
                if (notification.EmailSent == false &&
                    notification.AboveOrUnder.Equals("under") &&
                    notification.ActualValue <= notification.Value)
                {
                    notificationsToSend.Add(notification);
                }
            }

            //send those users an email about their notification
            foreach (Notification notification in notificationsToSend)
            {
                MessageService.ComposeNotificationEmail(notification);
                notification.EmailSent = true;
                using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
                {
                    context.Entry(notification).Property("EmailSent").IsModified = true;
                    await context.SaveChangesAsync();
                }
            }
        }

        public static async Task<List<Notification>> GetNotificationsAsync(int? id)
        {
            List<Notification> notifications = new List<Notification>();
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                if (id == null)
                {
                    notifications = await context.Notifications.Include(m => m.User).ToListAsync();
                }
                else
                {
                    notifications = await context.Notifications.Include(m => m.User).Where
                        (notificationsToRead => notificationsToRead.User.ID == id).ToListAsync();
                }
            }

            //I have to show the actual value for each note 
            foreach (Notification notification in notifications)
            {
                Conversion conversion = new Conversion();
                conversion.BaseCurrency = notification.BaseCurrency;
                conversion.EndCurrency = notification.EndCurrency;
                conversion.Amount = notification.Value;
                notification.ActualValue = CurrencyApiService.GetRate(conversion);
            }
            return notifications;
        }
    }
}
