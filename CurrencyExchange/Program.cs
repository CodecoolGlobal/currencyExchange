using CurrencyExchange.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            //TransferService.SendMoney(7, 28, "USD", 100);
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
