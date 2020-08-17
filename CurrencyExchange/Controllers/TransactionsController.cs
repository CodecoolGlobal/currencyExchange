using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.AspNetCore.Http;
using CurrencyExchange.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CurrencyExchange.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly List<string> currencies;
        private readonly CurrencyExchangeContext _context;

        public TransactionsController(CurrencyExchangeContext context)
        {
            currencies = CurrencyApiService.GetCurrencies();
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Transactions.ToListAsync());
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
        public IActionResult Create()
        {
            ViewBag.Currencies = currencies;
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Currency,Amount")] Transaction transaction, string RecipientEmail)
        {
            if (ModelState["Currency"].ValidationState.Equals(ModelValidationState.Valid) &&
                ModelState["Amount"].ValidationState.Equals(ModelValidationState.Valid))
            {
                try
                {
                    User recipient = _context.Users.Where(u => u.Email == RecipientEmail).First();
                    transaction.Recipient = recipient;
                }
                catch (InvalidOperationException e)
                {
                    Alert(e, RecipientEmail);
                    return View();
                }
                int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
                User sender = _context.Users.Where(u => u.ID == userIdFromSession).First();
                transaction.Sender = sender;
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                TransferService.SendMoney(transaction);
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.ID == id);
        }

        private void Alert(Exception e, string address)
        {
            ViewBag.Alert = $"Email address {address} is not registered in Currency Exchange!";
        }
    }
}
