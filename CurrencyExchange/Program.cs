using CurrencyExchange.Data;
using CurrencyExchange.Models;
using CurrencyExchange.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils.Messaging;
using System.Linq;

namespace CurrencyExchange
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            MessageService.Initialize();
            NotificationService.Initialize(services);
            BalanceService.Initialize(services);
            TransferService.Initialize(services);
            
            //using (var context = new CurrencyExchangeContext(
            //        services.GetRequiredService<
            //            DbContextOptions<CurrencyExchangeContext>>()))
            //{
            //    User sender = context.Users
            //        .Where(u => u.ID == 7)
            //        .First();
            //    User recipient = context.Users
            //        .Where(u => u.ID == 28)
            //        .First();
            //    TransferService.SendMoney(new Transaction() { Sender = sender, Recipient = recipient, Currency = "USD", Amount = 100 });
            //}
            
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
