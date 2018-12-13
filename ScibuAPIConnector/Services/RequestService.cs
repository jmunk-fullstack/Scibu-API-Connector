﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScibuAPIConnector.Extensions;

namespace ScibuAPIConnector.Services
{
    public class RequestService
    {
        public string Url = "http://api.scibu.com/api/";

        public bool AlreadyExist(string endpoint, string postData)
        {

            var postDataJObject = JObject.Parse(postData);
            var jsonPostData = "";

            if (endpoint == "company")
            {
                foreach (var data in UploadSettings.Companies)
                {
                    var splittedFilter = "ExternalId";
                    if (!postDataJObject.Properties()
                        .Where(property => string.Equals(property.Name, splittedFilter,
                            StringComparison.CurrentCultureIgnoreCase)).Any(property =>
                            property.Value.ToString().Equals(data[1].ToString()))) continue;

                    Console.WriteLine("Company already exist!");
                    return true;
                }
            }
            if (endpoint == "quote")
            {
                foreach (var data in UploadSettings.Quotes)
                {
                    var splittedFilter = "ExternalId";
                    if (!postDataJObject.Properties()
                        .Where(property => string.Equals(property.Name, splittedFilter,
                            StringComparison.CurrentCultureIgnoreCase)).Any(property =>
                            property.Value.ToString().Equals(data[1].ToString()))) continue;

                    Console.WriteLine("Quote already exist!");
                    return true;
                }
            }
            if (endpoint == "order")
            {
                foreach (var data in UploadSettings.Orders)
                {
                    var splittedFilter = "OrderName";
                    if (!postDataJObject.Properties()
                        .Where(property => string.Equals(property.Name, splittedFilter,
                            StringComparison.CurrentCultureIgnoreCase)).Any(property =>
                            property.Value.ToString().Equals(data[1].ToString()))) continue;

                    Console.WriteLine("Order already exist!");
                    return true;
                }
            }
            if (endpoint == "invoice")
            {
                foreach (var data in UploadSettings.Invoices)
                {
                    var splittedFilter = "InvoiceName";
                    if (!postDataJObject.Properties()
                        .Where(property => string.Equals(property.Name, splittedFilter,
                            StringComparison.CurrentCultureIgnoreCase)).Any(property =>
                            property.Value.ToString().Equals(data[1].ToString()))) continue;

                    Console.WriteLine("Invoice already exist!");
                    return true;
                }
            }

            return false;
        }
        

        public string PostRequest(string endpoint, string postData)
        {
            if (AlreadyExist(endpoint, postData))
                return "-1";

            var encoding = new System.Text.UTF8Encoding();
            var postDataBytes = encoding.GetBytes(postData);
            var webRequest = WebRequest.Create(Url + endpoint);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.ContentLength = postDataBytes.Length;
            webRequest.Headers.Add("Authorization", UploadSettings.Token.TokenType + " " + UploadSettings.Token.AccessToken);

            using (var stream = webRequest.GetRequestStream())
            {
                stream.Write(postDataBytes, 0, postDataBytes.Length);
            }

            using (var resp = WebResponseExtension.BetterGetResponse(webRequest))
            {
                HttpStatusCode wRespStatusCode;
                try
                {
                    HttpWebResponse wResp = (HttpWebResponse) resp;
                    wRespStatusCode = wResp.StatusCode;
                }
                catch (WebException we)
                {
                    wRespStatusCode = ((HttpWebResponse) we.Response).StatusCode;
                }
                
                var responseString = new StreamReader(resp.GetResponseStream()).ReadToEnd();
                    if (wRespStatusCode == HttpStatusCode.Created)
                {
                    LoggingService.Logging(UploadSettings.DatabaseName, UploadSettings.UploadType, UploadSettings.UploadName, "Added " + UploadSettings.UploadCall + " to the database.", wRespStatusCode.ToString(), UploadSettings.UploadCall);
                   
                    var data = JObject.Parse(responseString);
                    var jsonResult = "";
                    
                    foreach (var property in data.Properties())
                    {
                        jsonResult = property.Value.ToString();
                        break;
                    }
                    Console.WriteLine("Added " + endpoint + " with ID " + jsonResult);
                    return jsonResult;
                }
                else
                {
                    LoggingService.Logging(UploadSettings.DatabaseName, UploadSettings.UploadType, UploadSettings.UploadName, "Failed to add a " + UploadSettings.UploadCall + " to the database.", wRespStatusCode.ToString(), UploadSettings.UploadCall);
                    Console.WriteLine("Failed to add " + endpoint);
                    Console.WriteLine(responseString);
                    return "-1";
                }
            }
        }

        public string GetWebRequest(string endpoint, string customFilter = null, string topFilter = null, string jsonKey = null, string skipFilter = null)
        {
            var webRequest = WebRequest.Create(Url + endpoint);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("Authorization", UploadSettings.Token.TokenType + " " + UploadSettings.Token.AccessToken);
            if (customFilter != null)
                webRequest.Headers.Add("Filter", customFilter);
            if (topFilter != null)
                webRequest.Headers.Add("Top", topFilter);
            if (skipFilter != null)
                webRequest.Headers.Add("Skip", skipFilter);
            webRequest.Timeout = 7500000;

            var result = "";

            using (var resp = WebResponseExtension.BetterGetResponse(webRequest))
            {
                HttpStatusCode wRespStatusCode;
                try
                {
                    var wResp = (HttpWebResponse)resp;
                    wRespStatusCode = wResp.StatusCode;
                }
                catch (WebException we)
                {
                    wRespStatusCode = ((HttpWebResponse)we.Response).StatusCode;
                }

                result = new StreamReader(resp.GetResponseStream()).ReadToEnd();
            }

            if (jsonKey == null) return result;
            var jsonArray = JArray.Parse(result);

            var jsonResult = "0";

            if (jsonArray.Count == 0) return jsonResult;
            var data = JObject.Parse(jsonArray[jsonArray.Count - 1].ToString());
            jsonResult = data[jsonKey].ToString();

            return jsonResult;
        }

        public string GetRequest(string endpoint, string customFilter = null, string topFilter = null, string jsonKey = null)
        {
            var result = "";
            switch (endpoint)
            {
                case "company":
                {
                    foreach (var data in UploadSettings.Companies)
                    {
                        var splittedFilter = customFilter.Split('\'')[1];
                        if (!data[1].ToString().Contains(splittedFilter)) continue;
                        switch (jsonKey)
                        {
                            case null:
                                return data.ToString();
                            case "id":
                                return data[0].ToString();
                            case "externalId":
                                return data[1].ToString();
                            case "companyName":
                                return data[2].ToString();
                        }
                    }

                    return GetWebRequest(endpoint, customFilter, topFilter, jsonKey);
                }
                case "invoice":
                {
                    foreach (var data in UploadSettings.Invoices)
                    {
                        var splittedFilter = customFilter.Split('\'')[1];
                        if (!data[1].ToString().Contains(splittedFilter)) continue;
                        switch (jsonKey)
                        {
                            case null:
                                return data.ToString();
                            case "id":
                                return data[0].ToString();
                                case "InvoiceName":
                                return data[1].ToString();

                            }
                    }
                    
                    return GetWebRequest(endpoint, customFilter, topFilter, jsonKey);
                    }
                case "order":
                {
                    foreach (var data in UploadSettings.Orders)
                    {
                        var splittedFilter = customFilter.Split('\'')[1];
                        if (!data[1].ToString().Contains(splittedFilter)) continue;
                        switch (jsonKey)
                        {
                            case null:
                                return data.ToString();
                            case "id":
                                return data[0].ToString();
                            case "orderName":
                                return data[1].ToString();

                        }
                    }

                    return GetWebRequest(endpoint, customFilter, topFilter, jsonKey);
                }
                case "quote":
                {
                    foreach (var data in UploadSettings.Quotes)
                    {
                        var splittedFilter = customFilter.Split('\'')[1];
                        if (!data[1].ToString().Contains(splittedFilter)) continue;
                        switch (jsonKey)
                        {
                            case null:
                                return data.ToString();
                            case "id":
                                return data[0].ToString();
                            case "externalId":
                                return data[1].ToString();

                        }
                    }

                    return GetWebRequest(endpoint, customFilter, topFilter, jsonKey);
                }
                default:
                    result = GetRequest(endpoint, customFilter, topFilter, jsonKey);
                    break;
            }

            return result;
        }
    }
}