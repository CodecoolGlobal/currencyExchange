using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.AspNetCore.Http;
using CurrencyExchange.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using CurrencyExchange.Tools;

namespace CurrencyExchange.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly List<string> currencies;
        private readonly CurrencyExchangeContext _context;

        public TransactionsController(CurrencyExchangeContext context)
        {
            currencies = CurrencyApiTools.GetCurrencies();
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index(int? id)
        {
            int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
            if (userIdFromSession == id)
            {
                List<Transaction> transactions = await TransactionTools.GetTransactionsAsync(id, false);
                return View(transactions);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.ID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create(string? currency)
        {
            ViewBag.Currencies = currencies;
            ViewBag.PreSelect = currency;
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Currency,Amount")] Transaction transaction, string RecipientEmail, string NowOrLater, DateTime date)
        {
            if (ModelState["Currency"].ValidationState.Equals(ModelValidationState.Valid) &&
                ModelState["Amount"].ValidationState.Equals(ModelValidationState.Valid))
            {
                int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
                User sender = _context.Users.Where(u => u.ID == userIdFromSession).First();
                transaction.Sender = sender;

                if (!CurrencyOwned(transaction))
                {
                    AlertNoCurrency(transaction.Currency);
                    return View();
                }

                if (!EnoughMoney(transaction))
                {
                    AlertNotEnough(transaction.Currency);
                    return View();
                }

                try
                {
                    User recipient = _context.Users.Where(u => u.Email == RecipientEmail).First();
                    transaction.Recipient = recipient;
                }
                catch (InvalidOperationException e)
                {
                    AlertWrongEmail(e, RecipientEmail);
                    return View();
                }
                if (NowOrLater.Equals("now"))
                {
                    transaction.Date = DateTime.Now;
                    transaction.Status = Status.Completed;
                }
                else
                {
                    transaction.Date = date;
                    transaction.Status = Status.Pending;
                }

                _context.Add(transaction);
                await _context.SaveChangesAsync();
                if (transaction.Status == Status.Completed)
                {
                    TransferService.SendMoney(transaction);
                }
                return RedirectToAction("Index", new RouteValueDictionary(
                         new { controller = "Transactions", action = "Index", id = userIdFromSession })
                     );
            }
            return View(transaction);
        }

        public IActionResult Cancel(int id)
        {
            int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
            User user = _context.Users.Where(u => u.ID == userIdFromSession).First();
            Transaction transaction = TransactionTools.GetTransactionById(id);
            if(transaction.Sender.ID == user.ID)
            {
                if(transaction.Status == Status.Pending)
                {
                    transaction.Status = Status.Cancelled;
                    _context.Entry(transaction).Property("Status").IsModified = true;
                     _context.SaveChanges();
                }
            }
            return RedirectToAction("Index", new RouteValueDictionary(
                         new { controller = "Transactions", action = "Index", id = userIdFromSession })
                     );
        }


        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.ID == id);
        }

        private void AlertWrongEmail(Exception e, string address)
        {
            ViewBag.Alert = $"Email address {address} is not registered in Currency Exchange!";
        }

        private void AlertNoCurrency(string currency)
        {
            ViewBag.Alert = $"You dont have a {currency} balance!\n" +
                            $"Add {currency} first!";
            ViewBag.Currencies = currencies;
        }

        private void AlertNotEnough(string currency)
        {
            ViewBag.Alert = $"You dont have enough {currency} on your balance!\n" +
                           $"Add more {currency} to complete the transaction!";
            ViewBag.Currencies = currencies;
        }

        private bool CurrencyOwned(Transaction transaction)
        {
            return _context.Balances
                     .Include(b => b.User)
                     .Where(b => b.User == transaction.Sender)
                     .Where(b => b.Currency == transaction.Currency)
                     .Any();
        }

        private bool EnoughMoney(Transaction transaction)
        {
            return _context.Balances
                    .Include(b => b.User)
                    .Where(b => b.User == transaction.Sender)
                    .Where(b => b.Currency == transaction.Currency)
                    .Where(b => b.Amount >= transaction.Amount)
                    .Any();
        }
    }
}
