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
            DateTime startDate = GetRandomDate();
            DateTime endDate = GetRandomDate();
            while (startDate > endDate)
            {
                endDate = GetRandomDate();
            }
            string strStartDate = startDate.ToString("yyyy-MM-dd");
            string strEndDate = endDate.ToString("yyyy-MM-dd");
            string baseCurrency = "USD";
            string endCurrency = "HUF";

            var client = new RestClient($"https://api.exchangeratesapi.io/history?start_at={strStartDate}&end_at={strEndDate}&base={baseCurrency}&symbols={endCurrency}");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            JsonObject deserializedResponse = JsonConvert.DeserializeObject<JsonObject>(response.Content);
            JsonObject deserializedRates = JsonConvert.DeserializeObject<JsonObject>(deserializedResponse["rates"].ToString());
            var sortedRatesByDate = deserializedRates.OrderBy(d => d.Key).ToList();

            ViewBag.Rates = sortedRatesByDate;
            ViewBag.StartDate = strStartDate;
            ViewBag.EndDate = strEndDate;
            ViewBag.BaseCurrency = baseCurrency;
            ViewBag.EndCurrency = endCurrency;
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

        private DateTime GetRandomDate()
        {
            Random random = new Random();
            DateTime start = new DateTime(2000, 01, 01);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(random.Next(range));
        }

        private string GetRandomCurrency()
        {
            Random random = new Random();
            return "USD";
        }
    }
}
