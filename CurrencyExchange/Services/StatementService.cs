using CurrencyExchange.Models;
using CurrencyExchange.Tools;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyExchange.Services
{
    public class StatementService
    {
        private static readonly string TemplatePath = "./Resources/statement_template.pdf";

        public static async Task<string> ComposeStatementAsync(int id, int year, int month)
        {
            string FilePath = CreateFilePath(id, year, month);
            List<Transaction> transactions = await TransactionTools.GetTransactionsAsync(id, year, month);

            PdfDocument pdf = new PdfDocument();
            PdfDocument template = PdfReader.Open(TemplatePath, PdfDocumentOpenMode.Import);
            pdf.Info.Title = "Database to PDF";
            PdfPage pdfPage = pdf.AddPage(template.Pages[0]);

            XGraphics graph = XGraphics.FromPdfPage(pdfPage);
            XFont font = new XFont("Verdana", 10, XFontStyle.Regular);

            int yPoint = 100;

            foreach (Transaction transaction in transactions)
            {
                string header = $"transactions of {SQLTools.GetUserById(id).UserName} in {year}.{month}";
                string date = transaction.Date.ToString();
                string sender = transaction.Sender.UserName;
                string recipient = transaction.Recipient.UserName;
                string currency = transaction.Currency;
                string amount = transaction.Amount.ToString();

                graph.DrawString(header, font, XBrushes.Black, new XRect(220, 20, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                graph.DrawString(date, font, XBrushes.Black, new XRect(40, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                graph.DrawString(sender, font, XBrushes.Black, new XRect(220, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                graph.DrawString(recipient, font, XBrushes.Black, new XRect(300, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                graph.DrawString(currency, font, XBrushes.Black, new XRect(400, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                graph.DrawString(amount, font, XBrushes.Black, new XRect(470, yPoint, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                yPoint = yPoint + 40;
            }

            pdf.Save(FilePath);
            return FilePath;
        }

        private static string CreateFilePath(int id, int year, int month)
        {
            User user = SQLTools.GetUserById(id);
            string pdfFilename = $"./Resources/Statements/{user.UserName}_{year}_{month}.pdf";
            return pdfFilename;
        }
    }
}
