using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace CurrencyExchange.Services
{
    public class TransferService
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
            timer.Interval = 60000;

            timer.Elapsed += CheckTransactions;

            bool TestScheduledTransfer = true;

            timer.Enabled = false;

            if (TestScheduledTransfer)
            {
                timer.Enabled = true;
                timer.Start();
            }
        }

        public static void SendMoney(Transaction transaction)
        {
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                Balance senderBalance = context.Balances.Include(b => b.User)
                    .Where(b => b.User.ID == transaction.Sender.ID)
                    .Where(b => b.Currency == transaction.Currency).First();
                BalanceService.EditBalance(senderBalance, transaction.Amount * -1);

                try
                {
                    Balance recipientBalance = context.Balances.Include(b => b.User)
                        .Where(b => b.User.ID == transaction.Recipient.ID)
                        .Where(b => b.Currency == transaction.Currency).First();
                    BalanceService.EditBalance(recipientBalance, transaction.Amount);
                }
                catch (InvalidOperationException e)
                {
                    Balance newBalance = new Balance()
                    { User = transaction.Recipient, Currency = transaction.Currency, Amount = transaction.Amount };
                    BalanceService.AddNewBalance(newBalance);
                }
            }

            //send email to both parties
            MessageService.ComposeTransactionEmail(transaction);
        }

        private async static void CheckTransactions(object sender, EventArgs e)
        {
            List<Transaction> transactions = await GetTransactionsAsync(null, true);
            List<Transaction> transactionsToSend = new List<Transaction>();

            //check which transactions are scheduled to send now
            foreach (Transaction transaction in transactions)
            {
                if (transaction.Date.CompareTo(DateTime.Now) < 1)
                {
                    transactionsToSend.Add(transaction);
                }
            }

            //send those transactions
            foreach (Transaction transaction in transactionsToSend)
            {
                SendMoney(transaction);
                transaction.Status = "Completed";
                using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
                {
                    context.Entry(transaction).Property("Status").IsModified = true;
                    await context.SaveChangesAsync();
                }
            }
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
                if(checkPending)
                {
                    transactions = await context.Transactions
                        .Include(t => t.Sender)
                        .Include(t => t.Recipient)
                        .Where(t => t.Status == "Pending").ToListAsync();
                }
            }
            return transactions;
        }
    }
}
