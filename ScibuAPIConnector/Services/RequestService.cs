using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScibuAPIConnector.Extensions;

namespace ScibuAPIConnector.Services
{
    public class RequestService
    {
        public string Url = "http://api.scibu.com/api/";

        public int AlreadyExist(string endpoint, string postData)
        {
            int num2;
            JObject obj2 = JObject.Parse(postData);
            if (endpoint == "company")
            {
                using (IEnumerator<JToken> enumerator = UploadSettings.Companies.GetEnumerator())
                {
                    while (true)
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }
                        JToken current = enumerator.Current;
                        string str2 = "CustomerNumber";
                        int num = 3;
                        if (obj2[str2].ToString().Equals(current[num].ToString()))
                        {
                            Console.WriteLine("Company already exist!");
                            return int.Parse(current[0].ToString());
                        }
                    }
                }
            }
            if (endpoint != "contact")
            {
                if (endpoint != "tickets_calllog")
                {
                    if (endpoint == "quote")
                    {
                        using (IEnumerator<JToken> enumerator6 = UploadSettings.Quotes.GetEnumerator())
                        {
                            while (true)
                            {
                                if (!enumerator6.MoveNext())
                                {
                                    break;
                                }
                                JToken current = enumerator6.Current;
                                string str5 = "ExternalID";
                                if (obj2[str5].ToString().Equals(current[1].ToString()))
                                {
                                    Console.WriteLine("Quote already exist!");
                                    return int.Parse(current[0].ToString());
                                }
                            }
                        }
                    }
                    if (endpoint == "quotediscount")
                    {
                        using (IEnumerator<JToken> enumerator7 = UploadSettings.QuoteDiscounts.GetEnumerator())
                        {
                            while (true)
                            {
                                if (!enumerator7.MoveNext())
                                {
                                    break;
                                }
                                JToken current = enumerator7.Current;
                                string str6 = "QuoteId";
                                if (obj2[str6].ToString().Equals(current[1].ToString()))
                                {
                                    Console.WriteLine("Quote Discount already exist!");
                                    return int.Parse(current[0].ToString());
                                }
                            }
                        }
                    }
                    if (endpoint == "orderdiscount")
                    {
                        using (IEnumerator<JToken> enumerator8 = UploadSettings.OrderDiscounts.GetEnumerator())
                        {
                            while (true)
                            {
                                if (!enumerator8.MoveNext())
                                {
                                    break;
                                }
                                JToken current = enumerator8.Current;
                                string str7 = "OrderId";
                                string str8 = "DiscountCategory";
                                if (obj2[str7].ToString().Equals(current[1].ToString()) && obj2[str8].ToString().Equals(current[2].ToString()))
                                {
                                    Console.WriteLine("Order Discount already exist!");
                                    return int.Parse(current[0].ToString());
                                }
                            }
                        }
                    }
                    if (endpoint == "quote_product")
                    {
                        using (IEnumerator<JToken> enumerator9 = UploadSettings.QuoteProducts.GetEnumerator())
                        {
                            while (true)
                            {
                                if (!enumerator9.MoveNext())
                                {
                                    break;
                                }
                                JToken current = enumerator9.Current;
                                string str9 = "ArticleNumber";
                                string str10 = "TypeRowIndex";
                                if (UploadSettings.DatabaseName == "techneaportal")
                                {
                                    str10 = "QuoteId";
                                }
                                string str11 = "QuoteId";
                                if (UploadSettings.DatabaseName != "techneaportal")
                                {
                                    if (!((obj2[str9].ToString().Equals(current[1].ToString()) && obj2[str10].ToString().Equals(current[2].ToString())) && obj2[str11].ToString().Equals(current[3].ToString())))
                                    {
                                        continue;
                                    }
                                    Console.WriteLine("Quote already exist!");
                                    num2 = int.Parse(current[0].ToString());
                                }
                                else if (obj2[str9].ToString() == "")
                                {
                                    num2 = int.Parse(current[0].ToString());
                                }
                                else
                                {
                                    if (!(obj2[str9].ToString().Equals(current[1].ToString()) && obj2[str10].ToString().Equals(current[2].ToString())))
                                    {
                                        continue;
                                    }
                                    Console.WriteLine("Quote already exist!");
                                    num2 = int.Parse(current[0].ToString());
                                }
                                return num2;
                            }
                        }
                    }
                    if (endpoint == "invoice_product")
                    {
                        using (IEnumerator<JToken> enumerator10 = UploadSettings.InvoiceProducts.GetEnumerator())
                        {
                            while (true)
                            {
                                if (!enumerator10.MoveNext())
                                {
                                    break;
                                }
                                JToken current = enumerator10.Current;
                                string str12 = "ExternalId";
                                string str13 = "Order";
                                string str14 = "InvoiceId";
                                if ((obj2[str12].ToString().Equals(current[1].ToString()) && obj2[str13].ToString().Equals(current[2].ToString())) && obj2[str14].ToString().Equals(current[3].ToString()))
                                {
                                    Console.WriteLine("Invoice Product already exist!");
                                    return int.Parse(current[0].ToString());
                                }
                            }
                        }
                    }
                    if (endpoint == "order_product")
                    {
                        using (IEnumerator<JToken> enumerator11 = UploadSettings.OrderProducts.GetEnumerator())
                        {
                            while (true)
                            {
                                if (!enumerator11.MoveNext())
                                {
                                    break;
                                }
                                JToken current = enumerator11.Current;
                                string str15 = "Productnumber";
                                string str16 = "TypeRowIndex";
                                string str17 = "OrderId";
                                if ((obj2[str15].ToString().Equals(current[1].ToString()) && obj2[str16].ToString().Equals(current[2].ToString())) && obj2[str17].ToString().Equals(current[3].ToString()))
                                {
                                    Console.WriteLine("Order Product already exist!");
                                    return int.Parse(current[0].ToString());
                                }
                            }
                        }
                    }
                    if (endpoint == "order")
                    {
                        using (IEnumerator<JToken> enumerator12 = UploadSettings.Orders.GetEnumerator())
                        {
                            while (true)
                            {
                                if (!enumerator12.MoveNext())
                                {
                                    break;
                                }
                                JToken current = enumerator12.Current;
                                string str18 = "OrderName";
                                if (obj2[str18].ToString().Equals(current[1].ToString()))
                                {
                                    Console.WriteLine("Order already exist!");
                                    return int.Parse(current[0].ToString());
                                }
                            }
                        }
                    }
                    if (endpoint == "invoice")
                    {
                        using (IEnumerator<JToken> enumerator13 = UploadSettings.Invoices.GetEnumerator())
                        {
                            while (true)
                            {
                                if (!enumerator13.MoveNext())
                                {
                                    break;
                                }
                                JToken current = enumerator13.Current;
                                string str19 = "InvoiceName";
                                if (obj2[str19].ToString().Equals(current[1].ToString()))
                                {
                                    Console.WriteLine("Invoice already exist!");
                                    return int.Parse(current[0].ToString());
                                }
                            }
                        }
                    }
                    return -1;
                }
                else if (UploadSettings.Tickets != null)
                {
                    using (IEnumerator<JToken> enumerator4 = UploadSettings.Tickets.GetEnumerator())
                    {
                        while (true)
                        {
                            if (!enumerator4.MoveNext())
                            {
                                break;
                            }
                            JToken current = enumerator4.Current;
                            if (UploadSettings.DatabaseName == "hkvportal")
                            {
                                using (IEnumerator<JToken> enumerator5 = ((IEnumerable<JToken>)obj2["CUSTOMFIELDS"]).GetEnumerator())
                                {
                                    while (true)
                                    {
                                        if (enumerator5.MoveNext())
                                        {
                                            JToken token4 = enumerator5.Current;
                                            bool flag12 = token4["field"].ToString().Equals("UACTIVITYID");
                                            if (!flag12 || (token4["value"].ToString() != current[1].ToString()))
                                            {
                                                continue;
                                            }
                                            Console.WriteLine("Ticket already exist!");
                                            num2 = int.Parse(current[0].ToString());
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                        break;
                                    }
                                }
                                return num2;
                            }
                        }
                    }
                }
            }
            else
            {
                if (UploadSettings.Contacts != null)
                {
                    using (IEnumerator<JToken> enumerator2 = UploadSettings.Contacts.GetEnumerator())
                    {
                        while (true)
                        {
                            if (!enumerator2.MoveNext())
                            {
                                break;
                            }
                            JToken data = enumerator2.Current;
                            string splittedFilter = "ExternalId";
                            if ((from property in obj2.Properties()
                                 where string.Equals(property.Name, splittedFilter, StringComparison.CurrentCultureIgnoreCase)
                                 select property).Any<JProperty>(property => property.Value.ToString().Equals(data[1].ToString())))
                            {
                                if (UploadSettings.DatabaseName != "hkvportal")
                                {
                                    Console.WriteLine("Contact already exist!");
                                    num2 = int.Parse(data[0].ToString());
                                }
                                else
                                {
                                    using (IEnumerator<JToken> enumerator3 = ((IEnumerable<JToken>)obj2["CUSTOMFIELDS"]).GetEnumerator())
                                    {
                                        while (true)
                                        {
                                            if (enumerator3.MoveNext())
                                            {
                                                JToken current = enumerator3.Current;
                                                bool flag7 = current["field"].ToString().Equals("UCN_CONTACTPERSOON");
                                                if (!flag7 || (current["value"].ToString() != data[2].ToString()))
                                                {
                                                    continue;
                                                }
                                                Console.WriteLine("Contact already exist!");
                                                num2 = int.Parse(data[0].ToString());
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                            break;
                                        }
                                    }
                                }
                                return num2;
                            }
                        }
                    }
                }
                return -1;
            }
            return -1;
        }


        public string PostRequest(string endpoint, string postData, int hkvScibuId = 0)
        {
            string str;
            try
            {
                if (!endpoint.Contains("customfield"))
                {
                    int num = this.AlreadyExist(endpoint, postData);
                    if ((UploadSettings.DatabaseName == "hkvportal") && (hkvScibuId != 0))
                    {
                        num = hkvScibuId;
                    }
                    if (num != -1)
                    {
                        if (((endpoint != "contact") && ((endpoint != "company") && ((endpoint != "quote_product") && ((endpoint != "quote") && ((endpoint != "order") && ((endpoint != "order_product") && ((endpoint != "invoice") && (endpoint != "invoice_product")))))))) && (endpoint != "tickets_calllog"))
                        {
                            return "-1";
                        }
                        else if ((UploadSettings.LastDateUpdate == DateTime.Now.Date.ToShortDateString()) && (UploadSettings.DatabaseName != "hkvportal"))
                        {
                            return "-1";
                        }
                        else if ((endpoint != "tickets_calllog") || (num == -1))
                        {
                            endpoint = endpoint + "/" + num.ToString();
                        }
                        else
                        {
                            string str2 = num.ToString();
                            int length = str2.Length;
                            while (true)
                            {
                                if (length > 7)
                                {
                                    endpoint = endpoint + "/" + str2;
                                    break;
                                }
                                str2 = "0" + str2;
                                length++;
                            }
                        }
                    }
                    byte[] bytes = new UTF8Encoding().GetBytes(postData);
                    WebRequest request = WebRequest.Create(this.Url + endpoint);
                    request.Method = !endpoint.Contains("/") ? "POST" : "PUT";
                    request.ContentType = "application/json";
                    request.ContentLength = bytes.Length;
                    request.Headers.Add("Authorization", UploadSettings.Token.TokenType + " " + UploadSettings.Token.AccessToken);
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(bytes, 0, bytes.Length);
                    }
                    using (WebResponse response = request.BetterGetResponse())
                    {
                        HttpStatusCode statusCode;
                        try
                        {
                            statusCode = ((HttpWebResponse)response).StatusCode;
                        }
                        catch (WebException exception1)
                        {
                            statusCode = ((HttpWebResponse)exception1.Response).StatusCode;
                        }
                        string str3 = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        if (statusCode != HttpStatusCode.Created)
                        {
                            Console.WriteLine("Failed to add " + endpoint);
                            Console.WriteLine(str3);
                            Console.WriteLine(postData);
                            LogExtension.Log("Failed to add " + endpoint, DateTime.Now, postData, str3);
                            str = "-1";
                        }
                        else
                        {
                            string str4 = "";
                            using (IEnumerator<JProperty> enumerator = JObject.Parse(str3).Properties().GetEnumerator())
                            {
                                if (enumerator.MoveNext())
                                {
                                    str4 = enumerator.Current.Value.ToString();
                                }
                            }
                            string[] textArray1 = new string[] { "Added (", request.Method, ") ", endpoint, " with ID ", str4 };
                            Console.WriteLine(string.Concat(textArray1));
                            if (request.Method == "POST")
                            {
                                if (endpoint == "company")
                                {
                                    CacheService.AddToCacheCompanies(str3);
                                }
                                if (endpoint == "contact")
                                {
                                    CacheService.AddToCacheContacts(str3);
                                }
                                if (endpoint == "quote")
                                {
                                    CacheService.AddToCacheQuotes(str3);
                                }
                                if (endpoint == "quotediscount")
                                {
                                    CacheService.AddToCacheQuoteDiscount(str3);
                                }
                                if (endpoint == "quote_product")
                                {
                                    CacheService.AddToCacheQuoteProducts(str3);
                                }
                                if (endpoint == "invoice")
                                {
                                    CacheService.AddToCacheInvoices(str3);
                                }
                                if (endpoint == "invoice_product")
                                {
                                    CacheService.AddToCacheInvoiceProducts(str3);
                                }
                                if (endpoint == "order")
                                {
                                    CacheService.AddToCacheOrders(str3);
                                }
                                if (endpoint == "orderdiscount")
                                {
                                    CacheService.AddToCacheOrderDiscount(str3);
                                }
                                if (endpoint == "order_product")
                                {
                                    CacheService.AddToCacheOrderProducts(str3);
                                }
                            }
                            str = str4;
                        }
                    }
                }
                else
                {
                    str = "-1";
                }
            }
            catch (Exception exception3)
            {
                LogExtension.Log(exception3.Message, DateTime.Now, postData, "");
                str = "-1";
            }
            return str;
        }

        public string GetWebRequest(string endpoint, string customFilter = null, string topFilter = null, string jsonKey = null, string skipFilter = null, string cacheFilter = null)
        {
            string str3;
            WebRequest request = WebRequest.Create(this.Url + endpoint);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", UploadSettings.Token.TokenType + " " + UploadSettings.Token.AccessToken);
            if (customFilter != null)
            {
                request.Headers.Add("Filter", customFilter);
            }
            if (topFilter != null)
            {
                request.Headers.Add("Top", topFilter);
            }
            if (skipFilter != null)
            {
                request.Headers.Add("Skip", skipFilter);
            }
            if (cacheFilter != null)
            {
                request.Headers.Add("Cache", cacheFilter);
            }
            request.Timeout = 0x66851e0;
            string json = "";
            using (WebResponse response = request.BetterGetResponse())
            {
                HttpStatusCode statusCode;
                try
                {
                    statusCode = ((HttpWebResponse)response).StatusCode;
                }
                catch (WebException exception1)
                {
                    statusCode = ((HttpWebResponse)exception1.Response).StatusCode;
                }
                json = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            if (ReferenceEquals(jsonKey, null))
            {
                str3 = json;
            }
            else
            {
                JArray array = JArray.Parse(json);
                string str2 = "0";
                str3 = (array.Count != 0) ? JObject.Parse(array[array.Count - 1].ToString())[jsonKey].ToString() : str2;
            }
            return str3;
        }

        public string GetRequest(string endpoint, string customFilter = null, string topFilter = null, string jsonKey = null)
        {
            string str2 = endpoint;
            if (str2 != null)
            {
                string str7;
                if (str2 == "company")
                {
                    using (IEnumerator<JToken> enumerator = UploadSettings.Companies.GetEnumerator())
                    {
                        while (true)
                        {
                            if (!enumerator.MoveNext())
                            {
                                break;
                            }
                            JToken current = enumerator.Current;
                            char[] separator = new char[] { '\'' };
                            string str4 = customFilter.Split(separator)[1];
                            if (current[1].ToString().Contains(str4))
                            {
                                string str5 = jsonKey;
                                if (str5 == null)
                                {
                                    str7 = current.ToString();
                                }
                                else if (str5 == "id")
                                {
                                    str7 = current[0].ToString();
                                }
                                else if (str5 == "externalId")
                                {
                                    str7 = current[1].ToString();
                                }
                                else
                                {
                                    if (str5 != "companyName")
                                    {
                                        continue;
                                    }
                                    str7 = current[2].ToString();
                                }
                                return str7;
                            }
                        }
                    }
                    return this.GetWebRequest(endpoint, customFilter, topFilter, jsonKey, null, null);
                }
                else if (str2 == "invoice")
                {
                    using (IEnumerator<JToken> enumerator2 = UploadSettings.Invoices.GetEnumerator())
                    {
                        while (true)
                        {
                            if (!enumerator2.MoveNext())
                            {
                                break;
                            }
                            JToken current = enumerator2.Current;
                            char[] separator = new char[] { '\'' };
                            string str8 = customFilter.Split(separator)[1];
                            if (current[1].ToString().Contains(str8))
                            {
                                string str9 = jsonKey;
                                if (str9 == null)
                                {
                                    str7 = current.ToString();
                                }
                                else if (str9 == "id")
                                {
                                    str7 = current[0].ToString();
                                }
                                else
                                {
                                    if (str9 != "InvoiceName")
                                    {
                                        continue;
                                    }
                                    str7 = current[1].ToString();
                                }
                                return str7;
                            }
                        }
                    }
                    return this.GetWebRequest(endpoint, customFilter, topFilter, jsonKey, null, null);
                }
                else if (str2 == "order")
                {
                    using (IEnumerator<JToken> enumerator3 = UploadSettings.Orders.GetEnumerator())
                    {
                        while (true)
                        {
                            if (!enumerator3.MoveNext())
                            {
                                break;
                            }
                            JToken current = enumerator3.Current;
                            char[] separator = new char[] { '\'' };
                            string str11 = customFilter.Split(separator)[1];
                            if (current[1].ToString().Contains(str11))
                            {
                                string str12 = jsonKey;
                                if (str12 == null)
                                {
                                    str7 = current.ToString();
                                }
                                else if (str12 == "id")
                                {
                                    str7 = current[0].ToString();
                                }
                                else
                                {
                                    if (str12 != "orderName")
                                    {
                                        continue;
                                    }
                                    str7 = current[1].ToString();
                                }
                                return str7;
                            }
                        }
                    }
                    return GetWebRequest(endpoint, customFilter, topFilter, jsonKey, null, null);
                }
                else if (str2 == "quote")
                {
                    using (IEnumerator<JToken> enumerator4 = UploadSettings.Quotes.GetEnumerator())
                    {
                        while (true)
                        {
                            if (!enumerator4.MoveNext())
                            {
                                break;
                            }
                            JToken current = enumerator4.Current;
                            char[] separator = new char[] { '\'' };
                            string str14 = customFilter.Split(separator)[1];
                            if (current[1].ToString().Contains(str14))
                            {
                                string str15 = jsonKey;
                                if (str15 == null)
                                {
                                    str7 = current.ToString();
                                }
                                else if (str15 == "id")
                                {
                                    str7 = current[0].ToString();
                                }
                                else
                                {
                                    if (str15 != "externalId")
                                    {
                                        continue;
                                    }
                                    str7 = current[1].ToString();
                                }
                                return str7;
                            }
                        }
                    }
                    return GetWebRequest(endpoint, customFilter, topFilter, jsonKey, null, null);
                }
            }
            return GetRequest(endpoint, customFilter, topFilter, jsonKey);
        }
    }
}