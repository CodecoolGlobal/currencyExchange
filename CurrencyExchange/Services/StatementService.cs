using CurrencyExchange.Models;
using CurrencyExchange.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Services
{
    public class StatementService
    {
        public static async Task ComposeStatementAsync(int id, int year, int month)
        {
            string header = "Date:     Sender:     Recipient:     Currency:     Amount:";


            List<Transaction> transactions = await TransactionTools.GetTransactionsAsync(id, year, month);


            string Path = "./Resources/statement.txt";
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(Path))
            {
                sw.WriteLine(header);
                sw.WriteLine();
                foreach (Transaction transaction in transactions)
                {
                    string transactionLine = $"{transaction.Date}	{transaction.Sender.UserName}    {transaction.Recipient.UserName}    {transaction.Currency}    {transaction.Amount}";
                    sw.WriteLine(transactionLine);
                }
                sw.WriteLine();
                sw.WriteLine("Welcome");
            }
        }
    }
}
