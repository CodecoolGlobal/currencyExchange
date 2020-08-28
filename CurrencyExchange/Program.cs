using CurrencyExchange.Data;
using CurrencyExchange.Models;
using CurrencyExchange.Services;
using CurrencyExchange.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils.Messaging;
using System;
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
            InitServices(services);
            InitTools(services);
            host.Run();
        }

        private static void InitServices(IServiceProvider services)
        {
            MessageService.Initialize();
            NotificationService.Initialize(services);
            TransferService.Initialize(services);
        }

        private static void InitTools(IServiceProvider services)
        {
            BalanceTools.Initialize(services);
            NotificationTools.Initialize(services);
            TransactionTools.Initialize(services);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
