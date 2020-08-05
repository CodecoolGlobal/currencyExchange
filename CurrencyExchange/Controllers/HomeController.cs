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
using System.Text;
using Microsoft.AspNetCore.Http;

namespace CurrencyExchange.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly List<string> currencies;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            currencies = CurrencyApiFeatures.getCurrencies();
        }

        public async Task<IActionResult> IndexAsync()
        {
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

        public IActionResult ExchangeRate()
        {
            ViewBag.Currencies = currencies;
            return View();
        }

        [HttpPost]
        public IActionResult ExchangeRate(Conversion conversion)
        {
            ViewBag.Currencies = currencies;
            ViewBag.Response = CurrencyApiFeatures.GetRate(conversion);
            return View();
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
            ViewBag.Response = CurrencyApiFeatures.GetRate(conversion) * conversion.Amount;
            return View(conversion);
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
            int index = random.Next(0, baseCurrencies.Count -1);
            return baseCurrencies[index];
        }

    }
}
