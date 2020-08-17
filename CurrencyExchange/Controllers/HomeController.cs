using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CurrencyExchange.Models;
using RestSharp;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using CurrencyExchange.Services;
using Microsoft.AspNetCore.Routing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CurrencyExchange.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly List<string> currencies;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            currencies = CurrencyApiService.GetCurrencies();
        }

        public async Task<IActionResult> IndexAsync()
        {
            if(TempData["ConvertResponse"] != null)
            {
                ViewData["ConvertResponse"] = TempData["ConvertResponse"];
                ViewData["Amount"] = TempData["Amount"];
                ViewData["BaseCurrency"] = TempData["BaseCurrency"];
                ViewData["EndCurrency"] = TempData["EndCurrency"];
            }
            if (TempData["Response"] != null)
            {
                ViewData["Response"] = TempData["Response"];
            }
            ViewBag.Currencies = currencies;
            DateTime startDate = GetRandomDate();
            DateTime endDate = GetRandomDate();
            while (startDate > endDate)
            {
                endDate = GetRandomDate();
            }
            string strStartDate = startDate.ToString("yyyy-MM-dd");
            string strEndDate = endDate.ToString("yyyy-MM-dd");
            string baseCurrency = GetRandomCurrency();
            string endCurrency = "HUF";

            var client = new RestClient($"https://api.exchangeratesapi.io/history?start_at={strStartDate}&end_at={strEndDate}&base={baseCurrency}&symbols={endCurrency}");
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);

            JsonObject deserializedResponse = JsonConvert.DeserializeObject<JsonObject>(response.Content);
            JsonObject deserializedRates = JsonConvert.DeserializeObject<JsonObject>(deserializedResponse["rates"].ToString());
            var sortedRatesByDate = deserializedRates.OrderBy(d => d.Key).ToList();

            //get years and belonging points
            HashSet<string> years = new HashSet<string>();
            StringBuilder yearsString = new StringBuilder();
            StringBuilder DataPoints = new StringBuilder();

            //get selected points and convert tham into a string(we want to pass tham to javascript) 
            foreach (var item in sortedRatesByDate)
            {
                if (!years.Contains(item.Key.Substring(0, 7)))
                {
                    var values = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(item.Value.ToString());
                    foreach (var value in values.Values)
                    {
                        DataPoints.Append(Convert.ToDecimal(value).ToString() + "/");
                    }
                    years.Add(item.Key.Substring(0, 7));
                }
                else { continue; }
            }

            //convert selected years into a string
            foreach (var yeartring in years)
            {
                yearsString.Append(yeartring + ",");
            }

            //selected years in a correct format for javascript
            ViewBag.Years = yearsString.ToString().Substring(0, yearsString.ToString().Length - 1);
            //selected points in a correct format for javascript
            ViewBag.Data = DataPoints.ToString().Substring(0, DataPoints.ToString().Length - 1).Replace(",", ".");

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

        //public IActionResult ExchangeRate()
        //{
        //    ViewBag.Currencies = currencies;
        //    return View();
        //}


        public class Resp
        {
            public Resp(string name)
            {
                Name = name;
            }
            public string Name { get; set; }
        }

  
        public IActionResult ExchangeRate(Conversion conversion)
        {
            ViewBag.Currencies = currencies;
            String response = CurrencyApiService.GetRate(conversion).ToString();
            Resp resp = new Resp(response);
            string stringData = System.Text.Json.JsonSerializer.Serialize(resp);
            //TempData["Response"] = response;
            return Json(resp);
            //return RedirectToAction("Index", "Home");
        }

        public IActionResult ConvertMoney()
        {
            ViewBag.Currencies = currencies;
            return View();
        }

        [HttpPost]
        public IActionResult ConvertMoney(Conversion conversion)
        {
            ViewBag.Currencies = currencies;
            TempData["ConvertResponse"] = (CurrencyApiService.GetRate(conversion) * conversion.Amount).ToString();
            TempData["Amount"] = conversion.Amount.ToString();
            TempData["BaseCurrency"] = conversion.BaseCurrency.ToString();
            TempData["EndCurrency"] = conversion.EndCurrency.ToString();
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
            List<string> baseCurrencies = new List<string>() { "EUR", "USD", "CHF", "GBP" };
            int index = random.Next(0, baseCurrencies.Count - 1);
            return baseCurrencies[index];
        }
    }
}
