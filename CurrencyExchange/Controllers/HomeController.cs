using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CurrencyExchange.Models;
using RestSharp;
using RestSharp.Serialization.Json;
using Newtonsoft.Json;

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
            ViewBag.Response = GetRate(conversion);
            return View();
        }

        public IActionResult ConvertMoney()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConvertMoney(Conversion conversion)
        {
            ViewBag.Response = GetRate(conversion) * conversion.Amount;
            return View(conversion);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private decimal GetRate(Conversion conversion)
        {
            var client = new RestClient($"https://api.exchangeratesapi.io/latest?base={conversion.BaseCurrency}&symbols={conversion.EndCurrency}");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            JsonObject deserializedResponse = JsonConvert.DeserializeObject<JsonObject>(response.Content);
            JsonObject deserializedRates = JsonConvert.DeserializeObject<JsonObject>(deserializedResponse["rates"].ToString());
            decimal rate = Convert.ToDecimal(deserializedRates[conversion.EndCurrency]);
            return decimal.Round(rate, 3);
        }
    }
}
