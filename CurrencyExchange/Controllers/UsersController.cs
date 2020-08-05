using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;

namespace CurrencyExchange.Controllers
{
    public class UsersController : Controller
    {
        private readonly CurrencyExchangeContext _context;

        public UsersController(CurrencyExchangeContext context)
        {
            _context = context;
        }

        // GET: Users
        //[Authorize]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("sessionUserRole") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (HttpContext.Session.GetString("sessionUserRole").Equals("Admin"))
            {
                return View(await _context.Users.ToListAsync());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
            if (id == userIdFromSession)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(m => m.ID == id);
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Users/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("sessionUser") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (user.ConfirmPassword == null)
            {
                user.ConfirmPassword = user.Password;
            }
            if (ModelState["Email"].ValidationState.Equals(ModelValidationState.Valid) && ModelState["Password"].ValidationState.Equals(ModelValidationState.Valid))
            {
                User userFromDb = _context.Users.Where(userToRead => userToRead.Email == user.Email).First();
                if (userFromDb == null)
                {
                    return View();
                }
                bool passwordIsValid = BCrypt.Net.BCrypt.Verify(user.ConfirmPassword, userFromDb.Password);
                if (passwordIsValid)
                {
                    HttpContext.Session.SetString("sessionUser", userFromDb.ID.ToString());
                    HttpContext.Session.SetString("sessionUserRole", userFromDb.Role);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        // GET: Users/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: Users/Create
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("sessionUser") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("ID,Email,UserName,Password,ConfirmPassword")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Role = "User";
                try
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    Alert(e);
                    return View();
                }
                catch (DbUpdateException e)
                {
                    Alert(e);
                    return View();
                }
            }
            return Login(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
            if (id == userIdFromSession)
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Email,Password,UserName")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
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
            return View(user);
        }

        //Edit Email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(int id, [Bind("Email,ID")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState["Email"].ValidationState.Equals(ModelValidationState.Valid))
            {
                try
                {
                    //_context.Update(user);
                    _context.Entry(user).Property("Email").IsModified = true;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
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
            return View(user);
        }


        //Edit User Name
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserName(int id, [Bind("UserName,ID")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState["UserName"].ValidationState.Equals(ModelValidationState.Valid))
            {
                try
                {
                    //_context.Update(user);
                    _context.Entry(user).Property("UserName").IsModified = true;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
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
            return View(user);
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int userIdFromSession = Convert.ToInt32(HttpContext.Session.GetString("sessionUser"));
            if (id == userIdFromSession)
            {
                var user = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == id);
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }

        private void Alert(Exception e)
        {
            string InvalidColumn = "";
            string ExceptionMessage = e.InnerException.Message;
            if (ExceptionMessage.Contains("Email"))
            {
                InvalidColumn = "Email Address";
            }
            else
            {
                InvalidColumn = "Username";
            }
            string AlertMessage = InvalidColumn + " is already in use!";
            ViewBag.Alert = AlertMessage;
        }
    }
}
