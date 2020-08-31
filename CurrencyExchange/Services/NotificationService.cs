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
            bool TestEmailSending = true;

            if (TestEmailSending)
            {
                //interval is minutes
                Timer timer = TimerTools.GenerateTimer(1);
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
                if (notification.AboveOrUnder == AboveOrUnder.Above &&
                    notification.ActualValue >= notification.Value)
                {
                    notificationsToSend.Add(notification);
                }
                if (notification.AboveOrUnder == AboveOrUnder.Under &&
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
