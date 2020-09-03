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
    public class TransactionTools
    {
        private static IServiceProvider _serviceProvider;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static async Task<List<Transaction>> GetTransactionsAsync(int? id, bool checkPending)
        {
            List<Transaction> transactions = new List<Transaction>();
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                if (id != null)
                {
                    User user = context.Users.Find(id);
                    transactions = await context.Transactions
                        .Include(t => t.Sender)
                        .Include(t => t.Recipient)
                        .Where(t => t.Sender == user || t.Recipient == user).ToListAsync();
                }
                if (checkPending)
                {
                    transactions = await context.Transactions
                        .Include(t => t.Sender)
                        .Include(t => t.Recipient)
                        .Where(t => t.Status == Status.Pending).ToListAsync();
                }
            }
            return transactions;
        }

        public static Transaction GetTransactionById(int id)
        {
            Transaction transaction = new Transaction();
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                transaction = context.Transactions
                    .Include(t => t.Sender)
                    .Where(t => t.ID == id)
                    .First();
            }
            return transaction;
        }
    }
}
