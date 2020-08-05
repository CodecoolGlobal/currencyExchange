using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Data;
using CurrencyExchange.Models;
using RestSharp;
using Newtonsoft.Json;

namespace CurrencyExchange.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly List<string> currencies;
        private readonly CurrencyExchangeContext _context;

        public NotificationsController(CurrencyExchangeContext context)
        {
            currencies = getCurrencies();
            _context = context;
        }

        // GET: Notifications
        public async Task<IActionResult> Index(int id)
        {
            //I have to show the actual value for each note 
            List<Notification> notifications = await _context.Notifications.Where(notificationsToRead => notificationsToRead.User.ID == id).ToListAsync();
            foreach(var notification in notifications)
            {
                Conversion conversion = new Conversion();
                conversion.BaseCurrency = notification.BaseCurrency;
                conversion.EndCurrency = notification.EndCurrency;
                conversion.Amount = notification.Value;
                notification.ActualValue = GetRate(conversion);
            }
            return View(notifications);
        }


        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(m => m.ID == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: Notifications/Create
        public IActionResult Create()
        {
            ViewBag.Currencies = currencies;
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BaseCurrency,EndCurrency,Value,AboverOrUnder")] Notification notification, int id)
        {
            if (ModelState.IsValid)
            {
                User userFromDb = _context.Users.Where(userToRead => userToRead.ID == id).First();
                notification.User = userFromDb;
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,BaseCurrency,EndCurrency,Value,AboverOrUnder")] Notification notification)
        {
            if (id != notification.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(m => m.ID == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.ID == id);
        }



        public decimal GetRate(Conversion conversion)
        {
            var client = new RestClient($"https://api.exchangeratesapi.io/latest?base={conversion.BaseCurrency}&symbols={conversion.EndCurrency}");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            JsonObject deserializedResponse = JsonConvert.DeserializeObject<JsonObject>(response.Content);
            JsonObject deserializedRates = JsonConvert.DeserializeObject<JsonObject>(deserializedResponse["rates"].ToString());
            decimal rate = Convert.ToDecimal(deserializedRates[conversion.EndCurrency]);
            return decimal.Round(rate, 3);
        }


        private List<string> getCurrencies()
        {
            var client = new RestClient("https://api.exchangeratesapi.io/latest");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            //JsonDeserializer deserial = new JsonDeserializer();
            //var JSONObj = deserial.Deserialize<Dictionary<string, string>>(response);
            JsonObject ourlisting = JsonConvert.DeserializeObject<JsonObject>(response.Content);
            JsonObject ourlisting2 = JsonConvert.DeserializeObject<JsonObject>(ourlisting["rates"].ToString());
            List<string> currencyes = ourlisting2.Keys.ToList();
            currencyes.Add("EUR");

            return currencyes;
        }


    }
}
