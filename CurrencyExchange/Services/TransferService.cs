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

        public static void SendMoney(int senderId, int recipientId, string currency, int amount)
        {
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                Balance senderBalance = context.Balances.Include(b => b.User)
                    .Where(b => b.User.ID == senderId)
                    .Where(b => b.Currency == currency).First();
                BalanceService.EditBalance(senderBalance, amount * -1);

                Balance recipientBalance = context.Balances.Include(b => b.User)
                    .Where(b => b.User.ID == recipientId)
                    .Where(b => b.Currency == currency).First();
                BalanceService.EditBalance(recipientBalance, amount);
            }

            //send email to both parties
            MessageService.ComposeTransactionEmail(transaction);
        }
    }
}
