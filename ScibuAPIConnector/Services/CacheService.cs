using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScibuAPIConnector.Models;

namespace ScibuAPIConnector.Services
{
    public static class CacheService
    {

        public static void AddToCacheCompanies(string response)
        {
            JObject obj2 = JObject.Parse(response);
            JArray array1 = new JArray();
            array1.Add(obj2["id"]);
            array1.Add(obj2["externalId"]);
            array1.Add(obj2["companyName"]);
            array1.Add(obj2["customerNumber"]);
            JArray item = array1;
            UploadSettings.Companies.Add(item);
        }

        public static void AddToCacheContacts(string response)
        {
            JObject obj2 = JObject.Parse(response);
            JArray array1 = new JArray();
            array1.Add(obj2["id"]);
            array1.Add(obj2["externalId"]);
            JArray item = array1;
            if (UploadSettings.DatabaseName == "hkvportal")
            {
                foreach (JToken token in (IEnumerable<JToken>)obj2["customFields"])
                {
                    string str = token["field"].ToString();
                    if (str.Equals("UCN_Contactpersoon"))
                    {
                        item.Add(token["value"]);
                    }
                }
            }
            UploadSettings.Contacts.Add(item);
        }

        public static void AddToCacheInvoiceProducts(string response)
        {
            JObject obj2 = JObject.Parse(response);
            JArray array1 = new JArray();
            array1.Add(obj2["id"]);
            array1.Add(obj2["externalId"]);
            array1.Add(obj2["order"]);
            array1.Add(obj2["invoiceId"]);
            JArray item = array1;
            UploadSettings.InvoiceProducts.Add(item);
        }

        public static void AddToCacheInvoices(string response)
        {
            JObject obj2 = JObject.Parse(response);
            JArray array1 = new JArray();
            array1.Add(obj2["id"]);
            array1.Add(obj2["invoiceName"]);
            JArray item = array1;
            UploadSettings.Invoices.Add(item);
        }

        public static void AddToCacheOrderDiscount(string response)
        {
            JObject obj2 = JObject.Parse(response);
            JArray array1 = new JArray();
            array1.Add(obj2["id"]);
            array1.Add(obj2["orderId"]);
            array1.Add(obj2["discountCategory"]);
            JArray item = array1;
            UploadSettings.OrderDiscounts.Add(item);
        }

        public static void AddToCacheOrderProducts(string response)
        {
            JObject obj2 = JObject.Parse(response);
            JArray array1 = new JArray();
            array1.Add(obj2["id"]);
            array1.Add(obj2["productNumber"]);
            array1.Add(obj2["typeRowIndex"]);
            array1.Add(obj2["orderId"]);
            JArray item = array1;
            UploadSettings.OrderProducts.Add(item);
        }

        public static void AddToCacheOrders(string response)
        {
            JObject obj2 = JObject.Parse(response);
            JArray array1 = new JArray();
            array1.Add(obj2["id"]);
            array1.Add(obj2["orderName"]);
            JArray item = array1;
            UploadSettings.Orders.Add(item);
        }

        public static void AddToCacheQuoteDiscount(string response)
        {
            JObject obj2 = JObject.Parse(response);
            JArray array1 = new JArray();
            array1.Add(obj2["id"]);
            array1.Add(obj2["quoteId"]);
            JArray item = array1;
            UploadSettings.QuoteDiscounts.Add(item);
        }

        public static void AddToCacheQuoteProducts(string response)
        {
            JObject obj2 = JObject.Parse(response);
            JArray array1 = new JArray();
            array1.Add(obj2["id"]);
            array1.Add(obj2["articleNumber"]);
            array1.Add(obj2["typeRowIndex"]);
            array1.Add(obj2["quoteId"]);
            JArray item = array1;
            if (UploadSettings.DatabaseName == "techneaportal")
            {
                JArray array2 = new JArray();
                array2.Add(obj2["id"]);
                array2.Add(obj2["articleNumber"]);
                array2.Add(obj2["quoteId"]);
                item = array2;
            }
            UploadSettings.QuoteProducts.Add(item);
        }

        public static void AddToCacheQuotes(string response)
        {
            JObject obj2 = JObject.Parse(response);
            JArray array1 = new JArray();
            array1.Add(obj2["id"]);
            array1.Add(obj2["externalId"]);
            JArray item = array1;
            UploadSettings.Quotes.Add(item);
        }

        public static void AddToCacheTickets(string response)
        {
            JObject obj2 = JObject.Parse(response);
            JArray array1 = new JArray();
            array1.Add(obj2["callId"]);
            JArray item = array1;
            if (UploadSettings.DatabaseName == "hkvportal")
            {
                foreach (JToken token in (IEnumerable<JToken>)obj2["customFields"])
                {
                    string str = token["field"].ToString();
                    if (str.Equals("UACTIVITYID"))
                    {
                        item.Add(token["value"]);
                    }
                }
            }
            UploadSettings.Quotes.Add(item);
        }


        public static void CacheCompanies()
        {
            RequestService service = new RequestService();
            Console.WriteLine("Caching all the companies...");
            JArray array2 = new JArray();
            foreach (JToken token in JsonConvert.DeserializeObject<JArray>(service.GetWebRequest("company", null, "9999999", null, null, "[Id],[externalId],[CompanyName],[CustomerNumber]")))
            {
                JArray array1 = new JArray();
                array1.Add(token["id"]);
                array1.Add(token["externalId"]);
                array1.Add(token["companyName"]);
                array1.Add(token["customerNumber"]);
                JArray item = array1;
                array2.Add(item);
            }
            UploadSettings.Companies = array2;
            Console.WriteLine("Companies cached!");
        }

        public static void CacheContacts()
        {
            if (UploadSettings.UploadFiles.Any<string>(s => s.IndexOf("contact", StringComparison.CurrentCultureIgnoreCase) > -1))
            {
                RequestService service = new RequestService();
                Console.WriteLine("Caching all the contacts...");
                string str = "";
                str = (UploadSettings.DatabaseName != "hkvportal") ? service.GetWebRequest("contact", null, "9999999", null, null, "[Id],[ExternalId]") : service.GetWebRequest("contact", null, "9999999", null, null, "[Id],[ExternalId],[UCN_Contactpersoon]");
                JArray array2 = new JArray();
                using (IEnumerator<JToken> enumerator = JsonConvert.DeserializeObject<JArray>(str).GetEnumerator())
                {
                    JArray array3;
                    goto TR_0013;
                TR_0008:
                    array2.Add(array3);
                TR_0013:
                    while (true)
                    {
                        if (enumerator.MoveNext())
                        {
                            JToken current = enumerator.Current;
                            JArray array1 = new JArray();
                            array1.Add(current["id"]);
                            array1.Add(current["externalId"]);
                            array3 = array1;
                            if (UploadSettings.DatabaseName == "hkvportal")
                            {
                                foreach (JToken token2 in (IEnumerable<JToken>)current["customFields"])
                                {
                                    string str2 = token2["field"].ToString();
                                    if (str2.Equals("UCN_Contactpersoon"))
                                    {
                                        array3.Add(token2["value"]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            UploadSettings.Contacts = array2;
                            Console.WriteLine("Contacts cached!");
                            return;
                        }
                        break;
                    }
                    goto TR_0008;
                }
            }
        }

        public static void CacheInvoiceProducts()
        {
            RequestService service = new RequestService();
            if (UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("credit_regels", StringComparison.CurrentCultureIgnoreCase) > -1)) || UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("factuurregels", StringComparison.CurrentCultureIgnoreCase) > -1)))
            {
                Console.WriteLine("Caching all the invoice products...");
                JArray array2 = new JArray();
                var doneCaching = false;

                var count = 0;
                while (!doneCaching)
                {
                    var jsonArray = JsonConvert.DeserializeObject<JArray>(service.GetWebRequest("invoice_product", null, "100000", null, (100000 * count).ToString(), "[Id],[ExternalId],[Order],[InvoiceId]"));
                    if (jsonArray.Count == 0)
                        doneCaching = true;

                    foreach (JToken token in jsonArray)
                    {
                        JArray array1 = new JArray();
                        array1.Add(token["id"]);
                        array1.Add(token["externalId"]);
                        array1.Add(token["order"]);
                        array1.Add(token["invoiceId"]);
                        JArray item = array1;
                        array2.Add(item);
                    }

                    count++;
                }

                UploadSettings.InvoiceProducts = array2;
                Console.WriteLine("Invoice Products cached!");
            }
        }

        public static void CacheInvoices()
        {
            RequestService service = new RequestService();
            if ((UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("invoice", StringComparison.CurrentCultureIgnoreCase) > -1)) || UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("fact", StringComparison.CurrentCultureIgnoreCase) > -1))) || UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("credit", StringComparison.CurrentCultureIgnoreCase) > -1)))
            {
                Console.WriteLine("Caching all the invoices...");
                JArray array2 = new JArray();
                foreach (JToken token in JsonConvert.DeserializeObject<JArray>(service.GetWebRequest("invoice", null, "9999999", null, null, "[Id],[InvoiceName]")))
                {
                    JArray array1 = new JArray();
                    array1.Add(token["id"]);
                    array1.Add(token["invoiceName"]);
                    JArray item = array1;
                    array2.Add(item);
                }
                UploadSettings.Invoices = array2;
                Console.WriteLine("Invoices cached!");
            }
        }

        public static void CacheOrderDiscount()
        {
            RequestService service = new RequestService();
            if (UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("order", StringComparison.CurrentCultureIgnoreCase) > -1)) || UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("orderregels", StringComparison.CurrentCultureIgnoreCase) > -1)))
            {
                Console.WriteLine("Caching all the quote discount...");
                JArray array2 = new JArray();
                foreach (JToken token in JsonConvert.DeserializeObject<JArray>(service.GetWebRequest("orderDiscount", null, "9999999", null, null, "[Id],[OrderId],[DiscountCategory]")))
                {
                    JArray array1 = new JArray();
                    array1.Add(token["id"]);
                    array1.Add(token["orderId"]);
                    array1.Add(token["discountCategory"]);
                    JArray item = array1;
                    array2.Add(item);
                }
                UploadSettings.OrderDiscounts = array2;
                Console.WriteLine("Order Discount cached!");
            }
        }

        public static void CacheOrderProducts()
        {
            RequestService service = new RequestService();
            if (UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("orderregel", StringComparison.CurrentCultureIgnoreCase) > -1)) || UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("orderproduct", StringComparison.CurrentCultureIgnoreCase) > -1)))
            {
                Console.WriteLine("Caching all the order products...");
                JArray array2 = new JArray();
                var doneCaching = false;

                var count = 0;
                while (!doneCaching)
                {
                    var jsonArray = JsonConvert.DeserializeObject<JArray>(service.GetWebRequest("order_product", null, "100000", null, (100000 * count).ToString(), "[Id],[ProductNumber],[TypeRowIndex],[OrderId]"));
                    if (jsonArray.Count == 0)
                        doneCaching = true;

                    foreach (JToken token in jsonArray)
                    {
                        JArray array1 = new JArray();
                        array1.Add(token["id"]);
                        array1.Add(token["productNumber"]);
                        array1.Add(token["typeRowIndex"]);
                        array1.Add(token["orderId"]);
                        JArray item = array1;
                        array2.Add(item);
                    }

                    count++;
                }
                
                UploadSettings.OrderProducts = array2;
                Console.WriteLine("Order Products cached!");
            }
        }

        public static void CacheOrders()
        {
            RequestService service = new RequestService();
            if (UploadSettings.UploadFiles.Any<string>(s => s.IndexOf("order", StringComparison.CurrentCultureIgnoreCase) > -1))
            {
                Console.WriteLine("Caching all the orders...");
                JArray array2 = new JArray();
                foreach (JToken token in JsonConvert.DeserializeObject<JArray>(service.GetWebRequest("order", null, "9999999", null, null, "[Id],[OrderName]")))
                {
                    JArray array1 = new JArray();
                    array1.Add(token["id"]);
                    array1.Add(token["orderName"]);
                    JArray item = array1;
                    array2.Add(item);
                }
                UploadSettings.Orders = array2;
                Console.WriteLine("Orders cached!");
            }
        }

        public static void CacheQuoteDiscount()
        {
            RequestService service = new RequestService();
            if (UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("offerte", StringComparison.CurrentCultureIgnoreCase) > -1)) || UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("quoteproduct", StringComparison.CurrentCultureIgnoreCase) > -1)))
            {
                Console.WriteLine("Caching all the quote discount...");
                JArray array2 = new JArray();
                foreach (JToken token in JsonConvert.DeserializeObject<JArray>(service.GetWebRequest("quoteDiscount", null, "9999999", null, null, "[Id],[QuoteId]")))
                {
                    JArray array1 = new JArray();
                    array1.Add(token["id"]);
                    array1.Add(token["quoteId"]);
                    JArray item = array1;
                    array2.Add(item);
                }
                UploadSettings.QuoteDiscounts = array2;
                Console.WriteLine("Quote Discount cached!");
            }
        }

        public static void CacheQuoteProducts()
        {
            RequestService service = new RequestService();
            if (UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("offerteregel", StringComparison.CurrentCultureIgnoreCase) > -1)) || UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("offerte_regel", StringComparison.CurrentCultureIgnoreCase) > -1)))
            {
                Console.WriteLine("Caching all the invoice products...");
                JArray array2 = new JArray();
                var doneCaching = false;

                var count = 0;
                while (!doneCaching)
                {
                    var jsonArray = JsonConvert.DeserializeObject<JArray>(service.GetWebRequest("quote_product", null, "1000000", null, (100000 * count).ToString(), "[Id],[ArticleNumber],[TypeRowIndex],[RevisionId],[QuoteId]"));
                    if (UploadSettings.DatabaseName == "techneaportal")
                    {
                        jsonArray = JsonConvert.DeserializeObject<JArray>(service.GetWebRequest("quote_product", null, "1000000", null, (100000 * count).ToString(), null));
                    }
                    
                    if (jsonArray.Count == 0)
                        doneCaching = true;

                    foreach (JToken token in jsonArray)
                    {
                        JArray array1 = new JArray();
                        array1.Add(token["id"]);
                        array1.Add(token["articleNumber"]);
                        array1.Add(token["typeRowIndex"]);
                        array1.Add(token["quoteId"]);
                        JArray item = array1;
                        if (UploadSettings.DatabaseName == "techneaportal")
                        {
                            JArray array4 = new JArray();
                            array4.Add(token["id"]);
                            array4.Add(token["articleNumber"]);
                            array4.Add(token["quoteId"]);
                            item = array4;
                        }
                        array2.Add(item);
                    }

                    count++;
                }

                UploadSettings.QuoteProducts = array2;
                Console.WriteLine("Quote Products cached!");
            }
        }

        public static void CacheQuotes()
        {
            RequestService service = new RequestService();
            if (UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("offerte", StringComparison.CurrentCultureIgnoreCase) > -1)) || UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("quote", StringComparison.CurrentCultureIgnoreCase) > -1)))
            {
                Console.WriteLine("Caching all the quotes...");
                JArray array2 = new JArray();
                foreach (JToken token in JsonConvert.DeserializeObject<JArray>(service.GetWebRequest("quote", null, "9999999", null, null, "[Id],[ExternalId]")))
                {
                    JArray array1 = new JArray();
                    array1.Add(token["id"]);
                    array1.Add(token["externalId"]);
                    JArray item = array1;
                    array2.Add(item);
                }
                UploadSettings.Quotes = array2;
                Console.WriteLine("Quotes cached!");
            }
        }

        public static void CacheTickets()
        {
            RequestService service = new RequestService();
            if (UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("ticket", StringComparison.CurrentCultureIgnoreCase) > -1)) || UploadSettings.UploadFiles.Any<string>(s => (s.IndexOf("klacht", StringComparison.CurrentCultureIgnoreCase) > -1)))
            {
                Console.WriteLine("Caching all the tickets...");
                JArray array2 = new JArray();
                using (IEnumerator<JToken> enumerator = JsonConvert.DeserializeObject<JArray>(service.GetWebRequest("tickets_calllog", null, "9999999", null, null, "[Id],[ExternalId]")).GetEnumerator())
                {
                    JArray array3;
                    goto TR_0013;
                TR_0008:
                    array2.Add(array3);
                TR_0013:
                    while (true)
                    {
                        if (enumerator.MoveNext())
                        {
                            JToken current = enumerator.Current;
                            JArray array1 = new JArray();
                            array1.Add(current["callId"]);
                            array3 = array1;
                            if (UploadSettings.DatabaseName == "hkvportal")
                            {
                                foreach (JToken token2 in (IEnumerable<JToken>)current["customFields"])
                                {
                                    string str2 = token2["field"].ToString();
                                    if (str2.Equals("UACTIVITYID"))
                                    {
                                        array3.Add(token2["value"]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            UploadSettings.Tickets = array2;
                            Console.WriteLine("Tickets cached!");
                            return;
                        }
                        break;
                    }
                    goto TR_0008;
                }
            }
        }

        public static void Dispose()
        { 
            UploadSettings.Companies = null;
            UploadSettings.Invoices = null;
            UploadSettings.Quotes = null;
            UploadSettings.QuoteProducts = null;
            UploadSettings.QuoteDiscounts = null;
            UploadSettings.OrderDiscounts = null;
            UploadSettings.Orders = null;
            UploadSettings.Contacts = null;
            UploadSettings.OrderProducts = null;
            UploadSettings.Tickets = null;
            UploadSettings.InvoiceProducts = null;
        }
    }
}
