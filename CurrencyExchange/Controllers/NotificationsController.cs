﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Routing;

namespace CurrencyExchange.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly List<string> currencies;
        private readonly CurrencyExchangeContext _context;

        public NotificationsController(CurrencyExchangeContext context)
        {
            currencies = CurrencyApiService.getCurrencies();
            _context = context;
        }

        // GET: Notifications
        public async Task<IActionResult> Index(int id)
        {
            //I have to show the actual value for each note 
            List<Notification> notifications = await _context.Notifications.Where(notificationsToRead => notificationsToRead.User.ID == id).ToListAsync();
            foreach (var notification in notifications)
            {
                Conversion conversion = new Conversion();
                conversion.BaseCurrency = notification.BaseCurrency;
                conversion.EndCurrency = notification.EndCurrency;
                conversion.Amount = notification.Value;
                notification.ActualValue = CurrencyApiService.GetRate(conversion);
            }
            return View(notifications);
        }

        // GET: Notifications/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var notification = await _context.Notifications
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (notification == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(notification);
        //}

        // GET: Notifications/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("sessionUser") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Currencies = currencies;
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BaseCurrency,EndCurrency,Value,AboverOrUnder")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
                User userFromDb = _context.Users.Where(userToRead => userToRead.ID == userIdFromSession).First();
                notification.User = userFromDb;
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new RouteValueDictionary(
                        new { controller = "Notifications", action = "Index", id = userIdFromSession })
                    );
                //return Index(userIdFromSession);
            }
            return View(notification);
        }

        // GET: Notifications/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var notification = await _context.Notifications.FindAsync(id);
        //    if (notification == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(notification);
        //}

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ID,BaseCurrency,EndCurrency,Value,AboverOrUnder")] Notification notification)
        //{
        //    if (id != notification.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(notification);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!NotificationExists(notification.ID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(notification);
        //}

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));

            if (id == null)
            {
                return NotFound();
            }

            Notification notification = await _context.Notifications.Include(m => m.User).FirstOrDefaultAsync(m => m.ID == id);
            if (notification == null)
            {
                return NotFound();
            }

            //Notification newNoti = _context.Notifications.First(c => c.User.ID == userIdFromSession);
            //Course course = context.Courses.First(c => c.DepartmentID == 2);

            if (userIdFromSession != notification.User.ID)
            {
                return RedirectToAction("Index", "Home");
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
    }
}