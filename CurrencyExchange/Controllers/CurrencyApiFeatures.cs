using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyExchange.Models;
using Newtonsoft.Json;
using RestSharp;

namespace CurrencyExchange.Controllers
{
    public class CurrencyApiFeatures
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


        public static List<string> getCurrencies()
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
