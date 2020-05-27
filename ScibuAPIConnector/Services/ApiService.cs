using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ScibuAPIConnector.CustomFunctions;
using ScibuAPIConnector.Extensions;
using ScibuAPIConnector.Models;
using static System.Int32;

namespace ScibuAPIConnector.Services
{
    public class ApiService
    {
        private readonly string[] _customs = { "PHONE", "ADDRESS", "EMAIL", "CUSTOMFIELD" };
        private readonly string[] _customsSolo = { "USER", "QUOTEADDR", "INVOICEADDR", "ORDERADDR", "QUOTEOPPORTUNITY", "QUOTETEMPLATE", "ORDERTEMPLATE", "INVOICETEMPLATE" };
        private readonly string[] _cannotBeNull = { "PhoneNumber", "Street", "Mail" };

        public void AddCustomField(List<MappingRow> mappingRows)
        {
            foreach (MappingRow row in mappingRows)
            {
                Dictionary<string, object> customFieldDictionary = new Dictionary<string, object>();
                customFieldDictionary.Add("FieldName", row.ApiField);
                customFieldDictionary.Add("FieldLabel", "Custom field " + row.ApiField);
                customFieldDictionary.Add("Usage", row.Remark.Replace("DATATYPE=", ""));
                customFieldDictionary.Add("Widget1", "true");
                customFieldDictionary.Add("Widget2", "true");
                customFieldDictionary.Add("ShowOnFinancial", "true");
                customFieldDictionary.Add("ShowAlertBeforeExpiredDate", "true");
                customFieldDictionary.Add("AlertMessageBeforeExpiredDate", 0);
                customFieldDictionary.Add("AmountOfDaysBeforeExpiredDate", 0);
                customFieldDictionary.Add("AlertMessageAfterExpiredDate", 0);
                customFieldDictionary.Add("Widget3", "true");
                customFieldDictionary.Add("Widget4", "true");
                customFieldDictionary.Add("DisableFreeInput", "false");
                Dictionary<string, object> dictionary = customFieldDictionary;
                RequestService service = new RequestService();
                UploadSettings.UploadCall = row.ApiCall;
                service.PostRequest(mappingRows[0].ApiCall, JsonConvert.SerializeObject(dictionary), 0);
            }
        }


        public void AddToAPi(List<MappingRow> mappingRows, List<ImportTable> csvTables)
        {
            var authorizationService = new AuthorizationService();
            var token = UploadSettings.Token;

            var allPostData = new List<Dictionary<string, object>>();
            int hkvScibuId = 0;
            // Read all the CSV Tables
            foreach (var csvTable in csvTables)
            {
                //Read all the CSV Rows
                var i = 1;
                foreach (var csvRow in csvTable.Rows)
                {
                    try
                    {
                        Console.WriteLine("Mapping " + csvTable.ImportName + "... (" + i + " of " + csvTable.Rows.Count + ")");
                        i++;
                        //New POST data
                        var postData = new Dictionary<string, object>();

                        //Read the Mapping Rows one by one
                        foreach (var mappingRow in mappingRows)
                        {
                            var flag = false;

                            //Only check mappingRow of desired CSV Row
                            if (mappingRow.CsvName != csvTable.ImportName) continue;

                            //Find index of the CSV Row that is equal to the field in CSV and get the result of it
                            var search = mappingRow.FieldInCsv;
                            var index = -1;
                            if (mappingRow.FieldInCsv != "")
                            {
                                index = Array.FindIndex(csvTable.Columns,
                                    row => row.ToString() == search);
                            }

                            var result = "";

                            //If index = -1 then it is not found. 
                            if (index == -1)
                            {
                                //This will be executed when there needs to be a field in Scibu, but it is not in the import file
                                result = GetDefaultValue(result, mappingRow);

                                //Add Date to result
                                result = GetResultFromAddDate(result, mappingRow, csvTable, csvRow);

                                // Get Month/Year from row
                                result = GetResultFromDate(result, mappingRow, csvTable, csvRow);

                                //Get data from foreign key / primary key
                                result = GetResultFromKey(result, mappingRow, csvTable, csvRow, token);

                                result = GetResultFromGenderCheck(result, mappingRow, csvTable, csvRow);
                            }
                            else
                            {
                                result = csvRow[index];

                                if (mappingRow.Remark.Contains("CUSTOMFUNCTION"))
                                {
                                    var doneCustomFunction = false;
                                    if (mappingRow.Remark.Contains("TECHNEA"))
                                    {
                                        doneCustomFunction = Technea.TypeCheck(result, csvRow, csvTable.Columns, mappingRows, token);
                                        flag = true;
                                    }
                                    if (doneCustomFunction)
                                    {
                                        result = "REMOVE_CUSTOM_FUNCTION";
                                        flag = false;
                                    }
                                    if (mappingRow.Remark.Contains("HKV"))
                                    {
                                        hkvScibuId = int.Parse(result);
                                        result = "REMOVE_CUSTOM_FUNCTION";
                                        flag = true;
                                    }
                                }

                                if (mappingRow.Remark.Contains("FORMATDATE"))
                                {
                                    if (result.Contains("."))
                                    {
                                        result = DateTime.ParseExact(result, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                                    } else
                                    {
                                        result = (UploadSettings.DatabaseName != "hkvportal") ? ((result.Contains("-") || result.Contains("/")) ? DateTime.Parse(result).ToString("MM/dd/yyyy") : DateTime.ParseExact(result, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")) : ((result != "") ? DateTime.ParseExact(result, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy") : DateTime.ParseExact("01-01-2001", "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy"));
                                    }
                                }

                                if (mappingRow.Remark.Contains("GET_DATE") && ((UploadSettings.DatabaseName == "hkvportal") && (result != "")))
                                {
                                    result = DateTime.ParseExact(result, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                                }

                                if (mappingRow.Remark.Contains("GET_TIME") && ((result != "") && (UploadSettings.DatabaseName == "hkvportal")))
                                {
                                    result = DateTime.ParseExact(result, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("hh:ss");
                                }

                                if (mappingRow.Remark.Contains("EMPTY") && (result == ""))
                                {
                                    char[] separator = new char[] { '=' };
                                    result = mappingRow.Remark.Split(separator)[1];
                                }

                                // Replace Comma with a dot
                                if (mappingRow.Remark.Contains("COMMA_TO_DOT"))
                                {
                                    result = result.Replace(",", ".");
                                }

                                if (mappingRow.Remark.Contains("GET_STREET"))
                                {
                                    result = ReplaceNumbers(result);
                                }
                                if (mappingRow.Remark.Contains("GET_HOUSENUMBER"))
                                {
                                    result = GetNumbers(result);
                                }
                                if (mappingRow.Remark.Contains("BOOLEAN"))
                                {
                                    if (result == "1")
                                    {
                                        result = "true";
                                    }
                                    if (result == "0")
                                    {
                                        result = "false";
                                    }
                                }

                                // Custom IF statements in the remark
                                result = GetResultFromCustomIf(result, mappingRow, csvTable, csvRow, token);

                                // Merge two or more CSV rows in one API call
                                result = GetResultFromMerge(result, mappingRow, csvTable, csvRow, token);
                            }

                            //Add _CUSTOM to the result if it contains a custom
                            result = _customs.Where(custom => mappingRow.Remark.Contains(custom)).Aggregate(result,
                                (current, custom) => custom + "_" + current);
                            result = _customsSolo.Where(customSolo => mappingRow.Remark.Contains(customSolo)).Aggregate(result,
                                (current, customSolo) => customSolo + "_" + current);

                            if (!flag)
                            {
                                postData.Add(mappingRow.ApiField, result);
                            }
                        }

                        //Add the POST data to the queue
                        if (postData.Count != 0)
                        {
                            bool isRemovedCustomFunction = false;
                            foreach (object value in postData.Values)
                            {
                                if (value.ToString() == "REMOVE_CUSTOM_FUNCTION")
                                {
                                    isRemovedCustomFunction = true;
                                }
                            }
                            if (!isRemovedCustomFunction)
                            {
                                allPostData.Add(postData);
                            }
                        }

                        Console.WriteLine("Mapped " + csvTable.ImportName);
                    }
                    catch (Exception exception1)
                    {
                        LogExtension.Log(exception1.Message, DateTime.Now, "", "");
                    }
                }
            }

            var checkAllPostData = UpdateCustomsToPostData(allPostData);

            var count = 0;
            foreach (var updatedPostData in checkAllPostData)
            {
                count++;
                var requestService = new RequestService();
                UploadSettings.UploadCall = mappingRows[0].ApiCall;

                Console.WriteLine("Adding " + mappingRows[0].ApiCall + " (" + count + " of " + checkAllPostData.Count + ")");
                var id = Parse(requestService.PostRequest(mappingRows[0].ApiCall,
                    JsonConvert.SerializeObject(updatedPostData), hkvScibuId));
            }

            checkAllPostData = null;
            allPostData = null;
        }


        public List<Dictionary<string, object>> UpdateCustomsToPostData(List<Dictionary<string, object>> allPostData)
        {
            foreach (string str in this._customsSolo)
            {
                foreach (Dictionary<string, object> dictionary in allPostData.ToList<Dictionary<string, object>>())
                {
                    Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
                    foreach (string str2 in dictionary.Keys.ToList<string>())
                    {
                        if (dictionary[str2].ToString().Contains(str))
                        {
                            if (str != "USER")
                            {
                                dictionary2.Add(str2, dictionary[str2].ToString().Replace(str + "_", ""));
                                dictionary.Remove(str2);
                                continue;
                            }
                            Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
                            dictionary1.Add("username", dictionary[str2].ToString().Replace(str + "_", ""));
                            dictionary.Remove(str2);
                            dictionary.Add(str2, dictionary1);
                        }
                    }
                    if ((dictionary2.Count != 0) && (str != "USER"))
                    {
                        dictionary.Add(str, dictionary2);
                    }
                }
            }
            foreach (string str3 in this._customs)
            {
                foreach (Dictionary<string, object> dictionary4 in allPostData.ToList<Dictionary<string, object>>())
                {
                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    dictionary4.Add((str3 == "CUSTOMFIELD") ? "CUSTOMFIELDS" : str3, list);
                    foreach (string str4 in dictionary4.Keys.ToList<string>())
                    {
                        if (dictionary4[str4].ToString().Contains("CUSTOMFIELD") && str3.Equals("CUSTOMFIELD"))
                        {
                            Dictionary<string, object> dictionary10 = new Dictionary<string, object>();
                            dictionary10.Add("field", str4.Replace(str3 + "_", ""));
                            dictionary10.Add("value", dictionary4[str4].ToString().Replace(str3 + "_", ""));
                            list.Add(dictionary10);
                            dictionary4.Remove(str4);
                            continue;
                        }
                        if (dictionary4[str4].ToString().Contains(str3) && !str3.Equals("CUSTOMFIELD"))
                        {
                            string text = str4.Replace("_", "");
                            text = this.ReplaceNumbers(text);
                            if (!(str4.Contains("PhoneNumber_") || str4.Contains("AddressType_")))
                            {
                                item.Add(text, dictionary4[str4].ToString().Replace(str3 + "_", ""));
                            }
                            else
                            {
                                list.Add(item);
                                item = new Dictionary<string, object>();
                                item.Add(text, dictionary4[str4].ToString().Replace(str3 + "_", ""));
                            }
                            dictionary4.Remove(str4);
                        }
                    }
                    if (item.Count != 0)
                    {
                        list.Add(item);
                    }
                    else
                    {
                        dictionary4.Remove(str3);
                    }
                    string[] strArray3 = this._cannotBeNull;
                    int index = 0;
                    while (true)
                    {
                        if (index >= strArray3.Length)
                        {
                            if (list.Count == 0)
                            {
                                dictionary4.Remove((str3 == "CUSTOMFIELD") ? "CUSTOMFIELDS" : str3);
                            }
                            break;
                        }
                        string str6 = strArray3[index];
                        int num4 = 0;
                        foreach (Dictionary<string, object> dictionary7 in list.ToList<Dictionary<string, object>>())
                        {
                            foreach (KeyValuePair<string, object> pair in list[num4].ToList<KeyValuePair<string, object>>())
                            {
                                if ((pair.Key == str6) && ((pair.Value == null) || (((string)pair.Value) == "")))
                                {
                                    list.Remove(list[num4]);
                                    num4--;
                                }
                            }
                            num4++;
                        }
                        index++;
                    }
                }
            }
            foreach (Dictionary<string, object> dictionary8 in allPostData.ToList<Dictionary<string, object>>())
            {
                if (dictionary8.Count != 0)
                {
                    foreach (string str7 in dictionary8.Keys.ToList<string>())
                    {
                        if (str7.Count<char>(x => (x == '_')) == 2)
                        {
                            char[] separator = new char[] { '_' };
                            string key = str7.Split(separator)[1];
                            char[] chArray2 = new char[] { '_' };
                            string str9 = str7.Split(chArray2)[2];
                            object obj2 = dictionary8[str7];
                            dictionary8.Remove(str7);
                            foreach (Dictionary<string, object> dictionary9 in allPostData.ToList<Dictionary<string, object>>())
                            {
                                bool flag13 = false;
                                foreach (string str10 in dictionary9.Keys)
                                {
                                    if (str10 == str9)
                                    {
                                        flag13 = true;
                                    }
                                }
                                if (!flag13)
                                {
                                    allPostData.Remove(dictionary8);
                                }
                                else
                                {
                                    if (!dictionary9[str9].Equals(obj2))
                                    {
                                        continue;
                                    }
                                    if (!dictionary9.ContainsKey(key))
                                    {
                                        if (dictionary8.Count != 0)
                                        {
                                            dictionary9.Add(key, dictionary8[key]);
                                        }
                                        allPostData.Remove(dictionary8);
                                    }
                                    else
                                    {
                                        List<Dictionary<string, object>> list2 = (List<Dictionary<string, object>>)dictionary9[key];
                                        if (dictionary8.Count != 0)
                                        {
                                            list2.AddRange((List<Dictionary<string, object>>)dictionary8[key]);
                                        }
                                        dictionary9.Remove(key);
                                        allPostData.Remove(dictionary8);
                                        dictionary9.Add(key, list2);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return allPostData;
        }


        public string[] GetCustoms()
        {
            return _customs;
        }

        public string GetDefaultValue(string currentResult, MappingRow mappingRow)
        {
            string str2;
            string str = currentResult;
            if (!string.IsNullOrEmpty(mappingRow.FieldInCsv))
            {
                str2 = str;
            }
            else
            {
                str = !mappingRow.Remark.Contains("DEFAULT=EMPTY_STRING") ? (!mappingRow.Remark.Contains("DEFAULT=THIS.MONTH") ? (!mappingRow.Remark.Contains("DEFAULT=THIS.YEAR") ? (!mappingRow.Remark.Contains("DEFAULT=THIS.DATE") ? (!mappingRow.Remark.Contains("USER") ? mappingRow.DefaultValue : ("USER_" + mappingRow.DefaultValue)) : DateTime.Now.ToString("MM/dd/yyyy")) : DateTime.Now.Year.ToString()) : DateTime.Now.Month.ToString()) : "";
                str2 = str;
            }
            return str2;
        }

        private static string GetNumbers(string input) =>
    new string((from c in input
                where char.IsDigit(c)
                select c).ToArray<char>());

        public string GetResultFromDate(string currentResult, MappingRow mappingRow, ImportTable csvTable, string[] csvRow)
        {
            string str = currentResult;
            if (mappingRow.Remark.Contains("GETMONTH"))
            {
                char[] separator = new char[] { '=' };
                string key = mappingRow.Remark.Split(separator)[1];
                int index = Array.FindIndex<string>(csvTable.Columns, row => row.ToString() == key);
                str = (UploadSettings.DatabaseName != "hkvportal") ? DateTime.Parse(csvRow[index]).Month.ToString() : DateTime.ParseExact(csvRow[index], "dd-MM-yyyy", CultureInfo.InvariantCulture).Month.ToString();
            }
            if (mappingRow.Remark.Contains("GETYEAR"))
            {
                char[] separator = new char[] { '=' };
                string text1 = mappingRow.Remark.Split(separator)[1];
                int index = Array.FindIndex<string>(csvTable.Columns, row => row.ToString() == text1);
                str = (UploadSettings.DatabaseName != "hkvportal") ? DateTime.Parse(csvRow[index]).Year.ToString() : DateTime.ParseExact(csvRow[index], "dd-MM-yyyy", CultureInfo.InvariantCulture).Year.ToString();
            }
            return str;
        }

        public string GetResultFromGenderCheck(string currentResult, MappingRow mappingRow, ImportTable csvTable, string[] csvRow)
        {
            string str = currentResult;
            if (mappingRow.Remark.Contains("GENDER="))
            {
                char[] separator = new char[] { '=' };
                string key = mappingRow.Remark.Split(separator)[1];
                int index = Array.FindIndex<string>(csvTable.Columns, row => row.ToString() == key);
                string str2 = csvRow[index].ToString();
                str = !str2.Equals("de heer") ? (!str2.Equals("mevrouw") ? "Onbekend" : "Vrouw") : "Man";
            }
            if (mappingRow.Remark.Contains("TAV="))
            {
                char[] separator = new char[] { '=' };
                string text1 = mappingRow.Remark.Split(separator)[1];
                int index = Array.FindIndex<string>(csvTable.Columns, row => row.ToString() == text1);
                string str3 = csvRow[index].ToString();
                str = !str3.Equals("de heer") ? (!str3.Equals("mevrouw") ? "heer/mevrouw" : "mevrouw") : "de heer";
            }
            if (mappingRow.Remark.Contains("DEAR="))
            {
                char[] separator = new char[] { '=' };
                string text2 = mappingRow.Remark.Split(separator)[1];
                int index = Array.FindIndex<string>(csvTable.Columns, row => row.ToString() == text2);
                string str4 = csvRow[index].ToString();
                str = !str4.Equals("de heer") ? (!str4.Equals("mevrouw") ? "heer/mevrouw" : "mevrouw") : "heer";
            }
            return str;
        }

        public string GetResultFromAddDate(string currentResult, MappingRow mappingRow, ImportTable csvTable, string[] csvRow)
        {
            string str5;
            string str = currentResult;
            if (!mappingRow.Remark.Contains("ADD_DATE"))
            {
                str5 = str;
            }
            else
            {
                string[] separator = new string[] { "ADD_DATE:" };
                string[] strArray2 = mappingRow.Remark.Split(separator, StringSplitOptions.None);
                string field = "";
                string str2 = "";
                bool flag = false;
                if (strArray2[1].Contains("+"))
                {
                    char[] chArray1 = new char[] { '+' };
                    field = strArray2[1].Split(chArray1)[0];
                    char[] chArray2 = new char[] { '+' };
                    str2 = strArray2[1].Split(chArray2)[1];
                    flag = true;
                }
                else if (strArray2[1].Contains("-"))
                {
                    char[] chArray3 = new char[] { '-' };
                    field = strArray2[1].Split(chArray3)[0];
                    char[] chArray4 = new char[] { '-' };
                    str2 = strArray2[1].Split(chArray4)[1];
                }
                int index = Array.FindIndex<string>(csvTable.Columns, row => row.ToString().ToLower() == field.ToLower());
                if (index > csvRow.Length)
                {
                    str5 = currentResult;
                }
                else
                {
                    string s = csvRow[index];
                    DateTime time = new DateTime();
                    if (s.Contains("."))
                    {
                        time = DateTime.ParseExact(s, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        time = !s.Contains("-") ? DateTime.ParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture) : DateTime.ParseExact(s, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    }

                    
                    if (!str2.Contains("MONTH"))
                    {
                        str5 = str;
                    }
                    else
                    {
                        string str4 = str2.Replace("MONTH", "");
                        str5 = (flag ? time.AddMonths(int.Parse(str4)) : time.AddMonths(int.Parse(str4) * -1)).ToString("MM/dd/yyyy");
                    }
                }
            }
            return str5;
        }


        public string GetResultFromKey(string currentResult, MappingRow mappingRow, ImportTable csvTable, string[] csvRow, Token token)
        {
            string str6;
            string str = currentResult;
            if (!mappingRow.Remark.Contains("KEY"))
            {
                str6 = str;
            }
            else
            {
                string[] separator = new string[] { "KEY:" };
                char[] chArray1 = new char[] { '=' };
                string[] strArray2 = mappingRow.Remark.Split(separator, StringSplitOptions.None)[1].Split(chArray1);
                char[] chArray2 = new char[] { '&' };
                string[] strArray3 = strArray2[1].Split(chArray2);
                string foreignKey = strArray3[0];
                string str4 = strArray3[1];
                int index = Array.FindIndex<string>(csvTable.Columns, row => row.ToString() == foreignKey);
                string str5 = csvRow[index];
                string[] textArray2 = new string[] { "[", str4, "]='", str5, "'" };
                str6 = new RequestService().GetRequest(strArray2[0].ToLower(), string.Concat(textArray2), null, strArray2[2]);
            }
            return str6;
        }

        public string GetResultFromCustomIf(string currentResult, MappingRow mappingRow, ImportTable csvTable, string[] csvRow, Token token)
        {
            string str2;
            string str = currentResult;
            if (!mappingRow.Remark.Contains("IF"))
            {
                str2 = str;
            }
            else
            {
                string[] separator = new string[] { "IF:" };
                string[] strArray3 = mappingRow.Remark.Split(separator, StringSplitOptions.None);
                int index = 0;
                while (true)
                {
                    if (index >= strArray3.Length)
                    {
                        str2 = str;
                        break;
                    }
                    string str3 = strArray3[index];
                    if (str3.Contains("="))
                    {
                        char[] chArray1 = new char[] { '=' };
                        string str4 = str3.Replace(",", "").Split(chArray1)[0];
                        char[] chArray2 = new char[] { '=' };
                        string str5 = str3.Replace(",", "").Split(chArray2)[1];
                        if (str.ToUpper() == str4)
                        {
                            str = str5.Replace(" ", "");
                        }
                    }
                    index++;
                }
            }
            return str2;
        }

        public string ReplaceNumbers(string text) =>
            Regex.Replace(Regex.Replace(text, @"\d{2,}", ""), @"\d", "");

        public string GetResultFromMerge(string currentResult, MappingRow mappingRow, ImportTable csvTable, string[] csvRow, Token token)
        {
            string str = currentResult;
            if (!mappingRow.Remark.Contains("MERGE_WITH"))
            {
                if (mappingRow.Remark.Contains("MERGE"))
                {
                    str = "";
                    string[] separator = new string[] { "MERGE=" };
                    char[] chArray4 = new char[] { '&' };
                    foreach (string merge in mappingRow.Remark.Split(separator, StringSplitOptions.None)[1].Split(chArray4))
                    {
                        int index = Array.FindIndex<string>(csvTable.Columns, row => row.ToString() == merge);
                        bool flag4 = string.IsNullOrEmpty(str);
                        str = !flag4 ? (str + " " + csvRow[index]) : csvRow[index];
                    }
                }
            }
            else
            {
                str = "";
                char[] separator = new char[] { '(' };
                char[] chArray2 = new char[] { ')' };
                string str2 = mappingRow.Remark.Split(separator)[1].Split(chArray2)[0];
                string[] strArray = new string[] { ")=" };
                char[] chArray3 = new char[] { '&' };
                foreach (string text1 in mappingRow.Remark.Split(strArray, StringSplitOptions.None)[1].Split(chArray3))
                {
                    int index = Array.FindIndex<string>(csvTable.Columns, row => row.ToString() == text1);
                    bool flag2 = string.IsNullOrEmpty(str);
                    str = !flag2 ? (str + str2 + csvRow[index]) : csvRow[index];
                }
            }
            return str;
        }
    }
}