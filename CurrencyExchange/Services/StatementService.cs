using CurrencyExchange.Models;
using CurrencyExchange.Tools;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Services
{
    public class StatementService
    {
        public static async Task<Statement> ComposeStatementAsync(int id, int year, int month)
        {
            Statement statement = CreateStatement(id, year, month);
            List<Transaction> transactions = await TransactionTools.GetTransactionsAsync(id, year, month);
            
            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = "Database to PDF";
            PdfPage pdfPage = pdf.AddPage();
            XGraphics graph = XGraphics.FromPdfPage(pdfPage);
            XFont font = new XFont("Verdana", 10, XFontStyle.Regular);

            int yPoint = 100;

            foreach(Transaction transaction in transactions)
            {
                string date = transaction.Date.ToString();
                string sender = transaction.Sender.UserName;
                string recipient = transaction.Recipient.UserName;
                string currency = transaction.Currency;
                string amount = transaction.Amount.ToString();

                graph.DrawString(date, font, XBrushes.Black, new XRect(40, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                graph.DrawString(sender, font, XBrushes.Black, new XRect(230, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                graph.DrawString(recipient, font, XBrushes.Black, new XRect(280, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                graph.DrawString(currency, font, XBrushes.Black, new XRect(400, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                graph.DrawString(amount, font, XBrushes.Black, new XRect(440, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                yPoint = yPoint + 40;
            }

            pdf.Save(statement.FilePath);

            return statement;

            //string header = "Date:     Sender:     Recipient:     Currency:     Amount:";

            //string Path = "./Resources/statement.txt";
            //// Create a file to write to.
            //using (StreamWriter sw = File.CreateText(Path))
            //{
            //    sw.WriteLine(header);
            //    sw.WriteLine();
            //    foreach (Transaction transaction in transactions)
            //    {
            //        string transactionLine = $"{transaction.Date}	{transaction.Sender.UserName}    {transaction.Recipient.UserName}    {transaction.Currency}    {transaction.Amount}";
            //        sw.WriteLine(transactionLine);
            //    }
            //    sw.WriteLine();
            //    sw.WriteLine("Welcome");
            //}
        }

        private static Statement CreateStatement(int id, int year, int month)
        {
            User user = SQLTools.GetUserById(id);
            string pdfFilename = $"./Resources/Statements/{user.UserName}_{year}_{month}.pdf";

            Statement statement = new Statement()
            {
                User = user,
                StartDate = new DateTime(year, month, 1),
                EndDate = DateTime.Now,
                FilePath = pdfFilename
            };
            StatementTools.AddStatementToDb(statement);
            return statement;
        }
    }
}
