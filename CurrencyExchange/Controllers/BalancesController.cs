﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using CurrencyExchange.Tools;

namespace CurrencyExchange.Controllers
{
    public class BalancesController : Controller
    {
        private readonly List<string> currencies;
        private readonly CurrencyExchangeContext _context;

        public BalancesController(CurrencyExchangeContext context)
        {
            currencies = CurrencyApiTools.GetCurrencies();
            _context = context;
        }


        // GET: Balances
        public async Task<IActionResult> Index(int id)
        {
            int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
            if (userIdFromSession == id)
            {
                List<Balance> balances = await BalanceTools.GetBalancesAsync(id);
                return View(balances);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Balances/Create
        public IActionResult Create()
        {
            ViewBag.Currencies = currencies;
            return View();
        }

        // POST: Balances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Currency,Amount")] Balance balance)
        {
            if (ModelState.IsValid)
            {
                int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
                User userFromDb = SQLTools.GetUserById(userIdFromSession);

                //check if user has a balance with the same currency
                List<Balance> balances = await BalanceTools.GetBalancesAsync(userIdFromSession);
                bool AlreadyExists = false;
                foreach (Balance b in balances)
                {
                    if (b.Currency == balance.Currency)
                    {
                        BalanceTools.EditBalance(b, balance.Amount);
                        AlreadyExists = true;
                        break;
                    }
                }
                if (!AlreadyExists)
                {
                    balance.User = userFromDb;
                    _context.Balances.Add(balance);
                    await _context.SaveChangesAsync();
                    //BalanceService.AddNewBalance(balance);
                }
                return RedirectToAction("Index", new RouteValueDictionary(
                       new { controller = "Balances", action = "Index", id = userIdFromSession }));
            }
            return View(balance);
        }

        // GET: Balances/Add
        public async Task<IActionResult> AddAsync(int? id)
        {
            Balance balance = await _context.Balances
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.ID == id);
            int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
            if (userIdFromSession == balance.User.ID)
            {
                return View(balance);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Balances/Add
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, int amount)
        {
            Balance balance = await _context.Balances
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.ID == id);
            int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
            if (userIdFromSession == balance.User.ID)
            {
                BalanceTools.EditBalance(balance, amount);
                return RedirectToAction("Index", new RouteValueDictionary(
                       new { controller = "Balances", action = "Index", id = userIdFromSession }));
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ConvertMoneyAsync(int id)
        {
            Balance balance = await _context.Balances
               .Include(b => b.User)
               .FirstOrDefaultAsync(b => b.ID == id);
            ViewBag.Currencies = currencies;
            ViewBag.BaseCurrency = balance.Currency;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConvertMoneyAsync(Conversion conversion)
        {
            int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
            int amount = (int)((int)CurrencyApiTools.GetRate(conversion) * conversion.Amount);

            bool AlreadyExists = _context.Balances
                .Include(b => b.User)
                .Where(b => b.User.ID == userIdFromSession)
                .Where(b => b.Currency == conversion.EndCurrency)
                .Any();
            if (AlreadyExists)
            {
                Balance balance = await _context.Balances
                    .Include(b => b.User)
                    .Where(b => b.User.ID == userIdFromSession)
                    .Where(b => b.Currency == conversion.EndCurrency)
                    .FirstOrDefaultAsync();
                BalanceTools.EditBalance(balance, amount);
            }
            else
            {
                User user = SQLTools.GetUserById(userIdFromSession);
                Balance balance = new Balance() { Amount = amount, Currency = conversion.EndCurrency };
                balance.User = user;
                _context.Balances.Add(balance);
                await _context.SaveChangesAsync();
            }
            Balance original = await _context.Balances
                    .Include(b => b.User)
                    .Where(b => b.User.ID == userIdFromSession)
                    .Where(b => b.Currency == conversion.BaseCurrency)
                    .FirstOrDefaultAsync();
            BalanceTools.EditBalance(original, (int)conversion.Amount * -1);
            return RedirectToAction("Index", new RouteValueDictionary(
                       new
                       {
                           controller = "Balances",
                           action = "Index",
                           id = userIdFromSession
                       }));
        }

        private bool BalanceExists(int id)
        {
            return _context.Balances.Any(e => e.ID == id);
        }
    }
}

//// GET: Balances/Delete/5
//public async Task<IActionResult> Delete(int? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }

//    var balance = await _context.Balances
//        .FirstOrDefaultAsync(m => m.ID == id);
//    if (balance == null)
//    {
//        return NotFound();
//    }

//    return View(balance);
//}

// POST: Balances/Delete/5

//[HttpPost, ActionName("Delete")]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> DeleteConfirmed(int id)
//{
//    var balance = await _context.Balances.FindAsync(id);
//    _context.Balances.Remove(balance);
//    await _context.SaveChangesAsync();
//    return RedirectToAction(nameof(Index));
//}