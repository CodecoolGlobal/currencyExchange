using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Timers;

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
            timer.Enabled = true;
            timer.Start();
        }

        private async static void CheckNotifications(object sender, EventArgs e)
        {
            List<Notification> notifications = new List<Notification>();

            var context = new CurrencyExchangeContext(_serviceProvider.GetRequiredService<DbContextOptions<CurrencyExchangeContext>>());

            //notifications = await context.Notifications.ToListAsync();

            System.Diagnostics.Debug.WriteLine("time tick");
            foreach (var n in notifications)
            {
                System.Diagnostics.Debug.WriteLine(n.AboverOrUnder);
            }
        }
    }
}
