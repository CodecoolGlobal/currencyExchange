using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CurrencyExchange.Tools
{
    public class StatementTools
    {
        private static IServiceProvider _serviceProvider;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static async void AddStatementToDb(Statement statement)
        {
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                context.Add(statement);
                await context.SaveChangesAsync();
            }
        }

        public static string GetDateString(DateTime date)
        {
            string dateStr = date.Year.ToString();
            dateStr += ".";
            if(date.Month < 10)
            {
                dateStr += "0";
            }
            dateStr += date.Month.ToString();
            dateStr += ".";
            if (date.Day < 10)
            {
                dateStr += "0";
            }
            dateStr += date.Day.ToString();
            return dateStr;
        }
    }
}
