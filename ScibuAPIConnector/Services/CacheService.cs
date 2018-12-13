using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScibuAPIConnector.Services
{
    public static class CacheService
    {
        public static void CacheCompanies()
        {
            var requestService = new RequestService();

            Console.WriteLine("Caching all the companies...");
            var companies = requestService.GetWebRequest("company", null, "100000");
            var companyRequest = JsonConvert.DeserializeObject<JArray>(companies);
            var companyObj = new JArray();
            foreach (var req in companyRequest)
            {
                var newJsonObject = new JArray { req["id"], req["externalId"], req["companyName"] };
                companyObj.Add(newJsonObject);
            }
            UploadSettings.Companies = companyObj;
            Console.WriteLine("Companies cached!");
        }
        
        public static void CacheQuotes()
        {
            var requestService = new RequestService();
            if (!UploadSettings.UploadFiles.Any(s =>
                    s.IndexOf("offerte", StringComparison.CurrentCultureIgnoreCase) > -1) &&
                !UploadSettings.UploadFiles.Any(s => s.IndexOf("quote", StringComparison.CurrentCultureIgnoreCase) > -1)
            ) return;
            Console.WriteLine("Caching all the quotes...");
            var quotes = requestService.GetWebRequest("quote", null, "100000");
            var quoteRequest = JsonConvert.DeserializeObject<JArray>(quotes);
            var quoteObj = new JArray();
            foreach (var req in quoteRequest)
            {
                var newJsonObject = new JArray { req["id"], req["externalId"] };
                quoteObj.Add(newJsonObject);
            }

            UploadSettings.Quotes = quoteObj;
            Console.WriteLine("Quotes cached!");
        }
        
        public static void CacheInvoices()
        {
            var requestService = new RequestService();
            if (!UploadSettings.UploadFiles.Any(s =>
                    s.IndexOf("invoice", StringComparison.CurrentCultureIgnoreCase) > -1) &&
                !UploadSettings.UploadFiles.Any(s =>
                    s.IndexOf("fact", StringComparison.CurrentCultureIgnoreCase) > -1) &&
                !UploadSettings.UploadFiles.Any(s =>
                    s.IndexOf("credit", StringComparison.CurrentCultureIgnoreCase) > -1)
            ) return;

            Console.WriteLine("Caching all the invoices...");
            var invoices = requestService.GetWebRequest("invoice", null, "100000");
            var invoiceRequest = JsonConvert.DeserializeObject<JArray>(invoices);
            var invoiceObj = new JArray();
            foreach (var req in invoiceRequest)
            {
                var newJsonObject = new JArray { req["id"], req["invoiceName"] };
                invoiceObj.Add(newJsonObject);
            }

            UploadSettings.Invoices = invoiceObj;
            Console.WriteLine("Invoices cached!");
        }
        
        public static void CacheOrders()
        {
            var requestService = new RequestService();

            if (!UploadSettings.UploadFiles.Any(s => s.IndexOf("order", StringComparison.CurrentCultureIgnoreCase) > -1)
            ) return;

            Console.WriteLine("Caching all the orders...");
            var orders = requestService.GetWebRequest("order", null, "100000");
            var orderRequest = JsonConvert.DeserializeObject<JArray>(orders);
            var orderObj = new JArray();
            foreach (var req in orderRequest)
            {
                var newJsonObject = new JArray { req["id"], req["orderName"] };
                orderObj.Add(newJsonObject);
            }

            UploadSettings.Orders = orderObj;
            Console.WriteLine("Orders cached!");

        }
    }
}
