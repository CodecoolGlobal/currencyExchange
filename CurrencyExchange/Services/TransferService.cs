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
    public class TransferService
    {
        private static IServiceProvider _serviceProvider;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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

                Balance recipientBalance = context.Balances.Include(b => b.User)
                    .Where(b => b.User.ID == transaction.Recipient.ID)
                    .Where(b => b.Currency == transaction.Currency).First();
                BalanceService.EditBalance(recipientBalance, transaction.Amount);
            }

            //send email to both parties
            MessageService.ComposeTransactionEmail(transaction);
        }
    }
}
