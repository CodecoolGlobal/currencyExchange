using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CurrencyExchange.Models;
using RestSharp;

namespace CurrencyExchange.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ExchangeRate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ExchangeRate(Conversion conversion)
        {
            var client = new RestClient($"https://api.exchangeratesapi.io/latest?base={conversion.BaseCurrency}&symbols={conversion.EndCurrency}");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            ViewBag.Response = response.Content;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
