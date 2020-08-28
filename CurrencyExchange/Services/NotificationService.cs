using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Timers;
using CurrencyExchange.Tools;

namespace CurrencyExchange.Services
{
    public class NotificationService
    {
        private static IServiceProvider _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            bool TestEmailSending = false;

            if (TestEmailSending)
            {
                //interval is in miliseconds 
                //1000 units = 1 second
                //10000 = 10 seconds
                //60000 = 1 minute
                //3600000 = 1 hour
                Timer timer = TimerTool.GenerateTimer(10000);
                timer.Elapsed += CheckNotifications;
            }
        }

        private async static void CheckNotifications(object sender, EventArgs e)
        {
            List<Notification> notifications = await NotificationTools.GetNotificationsAsync(null, true, true);
            List<Notification> notificationsToSend = new List<Notification>();

            //check which notifications meet the criteria given by the users
            foreach (Notification notification in notifications)
            {
                if (notification.AboveOrUnder.Equals("above") &&
                    notification.ActualValue >= notification.Value)
                {
                    notificationsToSend.Add(notification);
                }
                if (notification.AboveOrUnder.Equals("under") &&
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
    }
}
