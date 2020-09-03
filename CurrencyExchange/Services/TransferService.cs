using CurrencyExchange.Data;
using CurrencyExchange.Models;
using CurrencyExchange.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace CurrencyExchange.Services
{
    public class TransferService
    {
        private static IServiceProvider _serviceProvider;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            bool TestScheduledTransfer = true;

            if (TestScheduledTransfer)
            {
                //interval is minutes
                Timer timer = TimerTools.GenerateTimer(1);
                timer.Elapsed += CheckTransactions;
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
                BalanceTools.EditBalance(senderBalance, transaction.Amount * -1);

                try
                {
                    Balance recipientBalance = context.Balances.Include(b => b.User)
                        .Where(b => b.User.ID == transaction.Recipient.ID)
                        .Where(b => b.Currency == transaction.Currency).First();
                    BalanceTools.EditBalance(recipientBalance, transaction.Amount);
                }
                catch (InvalidOperationException e)
                {
                    Balance newBalance = new Balance()
                    { User = transaction.Recipient, Currency = transaction.Currency, Amount = transaction.Amount };
                    BalanceTools.AddNewBalance(newBalance);
                }
            }

            //send email to both parties
            MessageService.ComposeTransactionEmail(transaction);
        }

        private async static void CheckTransactions(object sender, EventArgs e)
        {
            List<Transaction> transactions = await TransactionTools.GetTransactionsAsync(true);

            //send those transactions
            foreach (Transaction transaction in transactions)
            {
                SendMoney(transaction);
                transaction.Status = Status.Completed;
                using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
                {
                    context.Entry(transaction).Property("Status").IsModified = true;
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
