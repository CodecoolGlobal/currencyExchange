using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Tools
{
    public class NotificationTools
    {
        private static IServiceProvider _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static async Task<List<Notification>> GetNotificationsAsync(int? id, bool CurrencyNeeded, bool UnsentOnly)
        {
            List<Notification> notifications = new List<Notification>();
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                if (id == null)
                {
                    if (UnsentOnly)
                    {
                        notifications = await context.Notifications.Include(n => n.User)
                            .Where(n => n.EmailSent == false)
                            .ToListAsync();
                    }
                    else
                    {
                        notifications = await context.Notifications.Include(n => n.User)
                            .ToListAsync();
                    }
                }
                else
                {
                    notifications = await context.Notifications.Include(n => n.User)
                       .Where(n => n.User.ID == id)
                       .ToListAsync();
                }
            }

            //I have to show the actual value for each note
            if (CurrencyNeeded)
            {
                foreach (Notification notification in notifications)
                {
                    Conversion conversion = new Conversion();
                    conversion.BaseCurrency = notification.BaseCurrency;
                    conversion.EndCurrency = notification.EndCurrency;
                    conversion.Amount = notification.Value;
                    notification.ActualValue = CurrencyApiTools.GetRate(conversion);
                }
            }
            return notifications;
        }
    }
}
