﻿using CurrencyExchange.Data;
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

        public static async Task<List<Transaction>> GetTransactionsAsync(int? id)
        {
            List<Transaction> transactions = new List<Transaction>();
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                User user = context.Users.Find(id);
                transactions = await context.Transactions
                    .Include(t => t.Sender)
                    .Include(t => t.Recipient)
                    .Where(t => t.Sender == user || t.Recipient == user)
                    .OrderByDescending(t => t.Date).ToListAsync();
            }
            return transactions;
        }

        public static async Task<List<Transaction>> GetTransactionsAsync(bool checkPending)
        {
            List<Transaction> transactions = new List<Transaction>();
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                transactions = await context.Transactions
                        .Include(t => t.Sender)
                        .Include(t => t.Recipient)
                        .Where(t => t.Status == Status.Pending)
                        .Where(t => t.Date.CompareTo(DateTime.Now) < 1).ToListAsync();
            }
            return transactions;
        }

        public static async Task<List<Transaction>> GetTransactionsAsync(int id, int year, int month)
        {
            List<Transaction> transactions = new List<Transaction>();
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                User user = context.Users.Find(id);
                transactions = await context.Transactions
                    .Include(t => t.Sender)
                    .Include(t => t.Recipient)
                    .Where(t => t.Sender == user || t.Recipient == user)
                    .Where(t => t.Date.Year == year)
                    .Where(t => t.Date.Month == month)
                    .OrderByDescending(t => t.Date).ToListAsync();
            }
            return transactions;
        }
    }
}