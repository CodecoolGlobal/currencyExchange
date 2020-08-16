using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Services
{
    public class BalanceService
    {
        private static IServiceProvider _serviceProvider;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static async Task<List<Balance>> GetBalancesAsync(int id)
        {
            List<Balance> balances = new List<Balance>();
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                balances = await context.Balances.Include(b => b.User)
                    .Where(b => b.User.ID == id)
                    .ToListAsync();
            }
            return balances;
        }

        public static async void EditBalance(Balance balance, int amount)
        {
            balance.Amount += amount;
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                context.Entry(balance).Property("Amount").IsModified = true;
                await context.SaveChangesAsync();
            }
        }
    }
}
