using System;
using System.Collections.Generic;
using System.Linq;
using CurrencyExchange.Models;
using Newtonsoft.Json;
using RestSharp;

namespace CurrencyExchange.Tools
{
    public class CurrencyApiTools
    {
        public static decimal GetRate(Conversion conversion)
        {
            var client = new RestClient($"https://api.exchangeratesapi.io/latest?base={conversion.BaseCurrency}&symbols={conversion.EndCurrency}");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            JsonObject deserializedResponse = JsonConvert.DeserializeObject<JsonObject>(response.Content);
            JsonObject deserializedRates = JsonConvert.DeserializeObject<JsonObject>(deserializedResponse["rates"].ToString());
            decimal rate = Convert.ToDecimal(deserializedRates[conversion.EndCurrency]);

            return decimal.Round(rate, 3);
        }

        public static List<string> GetCurrencies()
        {
            var client = new RestClient("https://api.exchangeratesapi.io/latest");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            JsonObject ourlisting = JsonConvert.DeserializeObject<JsonObject>(response.Content);
            JsonObject ourlisting2 = JsonConvert.DeserializeObject<JsonObject>(ourlisting["rates"].ToString());
            List<string> currencies = ourlisting2.Keys.ToList();
            currencies.Add("EUR");

            return currencies;
        }
    }
}
