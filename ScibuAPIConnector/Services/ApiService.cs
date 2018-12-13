using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScibuAPIConnector.Models;
using static System.Int32;

namespace ScibuAPIConnector.Services
{
    public class ApiService
    {
        private readonly string[] _customs = {"PHONE", "ADDRESS", "EMAIL", "CUSTOMFIELD"};
        private readonly string[] _customsSolo = {"USER", "QUOTEADDR", "INVOICEADDR", "ORDERADDR", "QUOTEOPPORTUNITY"};
        private readonly string[] _cannotBeNull = {"PhoneNumber"};

        public List<Dictionary<string, object>> UpdateCustomsToPostData(List<Dictionary<string, object>> allPostData)
        {
            //One to one relations in a dictionary
            foreach (var customsSolo in _customsSolo)
            {
                foreach (var postData in allPostData.ToList())
                {
                    var customDictionary = new Dictionary<string, object>();
                    foreach (var key in postData.Keys.ToList())
                    {
                        if (!postData[key].ToString().Contains(customsSolo)) continue;

                        if (customsSolo == "USER")
                        {
                            var userDictionary = new Dictionary<string, object>
                            {
                                {"username", postData[key].ToString().Replace(customsSolo + "_", "")}
                            };
                            postData.Remove(key);
                            postData.Add(key, userDictionary);
                        }
                        else
                        {
                            customDictionary.Add(key, postData[key].ToString().Replace(customsSolo + "_", ""));
                            postData.Remove(key);
                        }
                    }

                    if (customDictionary.Count != 0 && customsSolo != "USER")
                    {
                        postData.Add(customsSolo, customDictionary);
                    }
                }
            }

            //One to one relations in an array
            foreach (var custom in _customs)
            {
                foreach (var postData in allPostData.ToList())
                {
                    var customsArray = new List<Dictionary<string, object>>();
                    var customsDictionary = new Dictionary<string, object>();

                    postData.Add(custom == "CUSTOMFIELD" ? "CUSTOMFIELDS" : custom, customsArray);

                    foreach (var key in postData.Keys.ToList())
                    {
                        //Replace the customfields
                        if (postData[key].ToString().Contains("CUSTOMFIELD") && custom.Equals("CUSTOMFIELD"))
                        {
                            var customfieldDictionary = new Dictionary<string, object>
                            {
                                {"field", key.Replace(custom + "_", "")},
                                {"value", postData[key].ToString().Replace(custom + "_", "")}
                            };
                            customsArray.Add(customfieldDictionary);
                            postData.Remove(key);
                        }
                        else if (postData[key].ToString().Contains(custom) && !custom.Equals("CUSTOMFIELD"))
                        {
                            customsDictionary.Add(key, postData[key].ToString().Replace(custom + "_", ""));
                            postData.Remove(key);
                        }
                    }

                    foreach (var checkNull in _cannotBeNull)
                    {
                        foreach (var customDic in customsDictionary)
                        {
                            if (customDic.Key != checkNull) continue;
                            if (customDic.Value != null && (string) customDic.Value != "") continue;
                            postData.Remove(custom);
                        }
                    }

                    if (customsDictionary.Count != 0)
                        customsArray.Add(customsDictionary);
                    else
                        postData.Remove(custom);


                    if (customsArray.Count == 0)
                        postData.Remove(custom == "CUSTOMFIELD" ? "CUSTOMFIELDS" : custom);
                }
            }

            foreach (var postData in allPostData.ToList())
            {
                foreach (var key in postData.Keys.ToList())
                {
                    if (key.Count(x => x == '_') != 2) continue;

                    var apiField = key.Split('_')[1];
                    var foreignKey = key.Split('_')[2];
                    var value = postData[key];
                    postData.Remove(key);

                    //Merge many to many relationships 
                    foreach (var postDataPrimary in allPostData.ToList())
                    {
                        if (!postDataPrimary[foreignKey].Equals(value)) continue;

                        if (postDataPrimary.ContainsKey(apiField))
                        {
                            var allCustoms = (List<Dictionary<string, object>>) postDataPrimary[apiField];
                            var newCustoms = (List<Dictionary<string, object>>) postData[apiField];
                            allCustoms.AddRange(newCustoms);

                            postDataPrimary.Remove(apiField);
                            allPostData.Remove(postData);
                            postDataPrimary.Add(apiField, allCustoms);
                            break;
                        }

                        postDataPrimary.Add(apiField, postData[apiField]);
                        allPostData.Remove(postData);
                        break;
                    }
                }
            }

            return allPostData;
        }

        public void AddToAPi(List<MappingRow> mappingRows, List<CsvTable> csvTables)
        {
            var authorizationService = new AuthorizationService();
            var token = UploadSettings.Token;

            var allPostData = new List<Dictionary<string, object>>();
            // Read all the CSV Tables
            foreach (var csvTable in csvTables)
            {
                //Read all the CSV Rows
                var i = 1;
                foreach (var csvRow in csvTable.Rows)
                {
                    Console.WriteLine("Mapping " + csvTable.CsvName + "... (" + i + " of " + csvTable.Rows.Count + ")");
                    i++;
                    //New POST data
                    var postData = new Dictionary<string, object>();

                    //Read the Mapping Rows one by one
                    foreach (var mappingRow in mappingRows)
                    {
                        //Only check mappingRow of desired CSV Row
                        if (mappingRow.CsvName != csvTable.CsvName) continue;

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
                        }
                        else
                        {
                            result = csvRow[index];

                            // Replace Comma with a dot
                            if (mappingRow.Remark.Contains("COMMA_TO_DOT"))
                            {
                                result = result.Replace(",", ".");
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

                        postData.Add(mappingRow.ApiField, result);
                    }

                    //Add the POST data to the queue
                    if (postData.Count != 0)
                        allPostData.Add(postData);

                    Console.WriteLine("Mapped " + csvTable.CsvName);
                }

            }

            var checkAllPostData = UpdateCustomsToPostData(allPostData);

            var count = 0;
            foreach (var updatedPostData in checkAllPostData)
            {
                count++;
                var requestService = new RequestService();
                UploadSettings.UploadCall = mappingRows[0].ApiCall;

                Console.WriteLine("Adding " + mappingRows[0].ApiCall + " (" + count + " of " + checkAllPostData.Count +")");
                var id = Parse(requestService.PostRequest(mappingRows[0].ApiCall,
                    JsonConvert.SerializeObject(updatedPostData)));
            }

            checkAllPostData = null;
            allPostData = null;
        }

        public void AddCustomField(List<MappingRow> mappingRows)
        {
            //Read the Mapping Rows one by one
            foreach (var mappingRow in mappingRows)
            {
                var postData = new Dictionary<string, object>
                {
                    {"FieldName", mappingRow.ApiField},
                    {"FieldLabel", "Custom field " + mappingRow.ApiField},
                    {"Usage", mappingRow.Remark.Replace("DATATYPE=", "")},
                    {"Widget1", "true"},
                    {"Widget2", "true"},
                    {"ShowOnFinancial", "true"},
                    {"ShowAlertBeforeExpiredDate", "true"},
                    {"AlertMessageBeforeExpiredDate", 0},
                    {"AmountOfDaysBeforeExpiredDate", 0},
                    {"AlertMessageAfterExpiredDate", 0},
                    {"Widget3", "true"},
                    {"Widget4", "true"},
                    {"DisableFreeInput", "false"}
                };

                var requestService = new RequestService();
                UploadSettings.UploadCall = mappingRow.ApiCall;
                requestService.PostRequest(mappingRows[0].ApiCall,
                    JsonConvert.SerializeObject(postData));
            }
        }

        public string[] GetCustoms()
        {
            return _customs;
        }

        public string GetDefaultValue(string currentResult, MappingRow mappingRow)
        {
            var result = currentResult;
            if (!string.IsNullOrEmpty(mappingRow.FieldInCsv)) return result;

            if (mappingRow.Remark.Contains("DEFAULT=EMPTY_STRING"))
                result = "";
            else if (mappingRow.Remark.Contains("DEFAULT=THIS.MONTH"))
                result = DateTime.Now.Month.ToString();
            else if (mappingRow.Remark.Contains("DEFAULT=THIS.YEAR"))
                result = DateTime.Now.Year.ToString();
            else if (mappingRow.Remark.Contains("DEFAULT=THIS.DATE"))
                result = DateTime.Now.ToString();
            else if (mappingRow.Remark.Contains("USER"))
                result = "USER_" + mappingRow.DefaultValue;
            else
                result = mappingRow.DefaultValue;

            return result;
        }

        public string GetResultFromDate(string currentResult, MappingRow mappingRow, CsvTable csvTable, string[] csvRow)
        {
            var result = currentResult;
            
            if (mappingRow.Remark.Contains("GETMONTH"))
            {
                var key = mappingRow.Remark.Split('=')[1];
                var indexKey = Array.FindIndex(csvTable.Columns,
                    row => row.ToString() == key);
                result = DateTime.Parse(csvRow[indexKey]).Month.ToString();
            }

            if (mappingRow.Remark.Contains("GETYEAR"))
            {
                var key = mappingRow.Remark.Split('=')[1];
                var indexKey = Array.FindIndex(csvTable.Columns,
                    row => row.ToString() == key);
                result = DateTime.Parse(csvRow[indexKey]).Year.ToString();
            }

            return result;
        }

        public string GetResultFromAddDate(string currentResult, MappingRow mappingRow, CsvTable csvTable, string[] csvRow)
        {
            var result = currentResult;

            if (!mappingRow.Remark.Contains("ADD_DATE")) return result;
            var stringSeparators = new[] { "ADD_DATE:" };
            var addStatements = mappingRow.Remark.Split(stringSeparators, StringSplitOptions.None);
            var field = "";
            var addValue = "";
            var isPlus = false;
            if (addStatements[1].Contains("+"))
            {
                field = addStatements[1].Split('+')[0];
                addValue = addStatements[1].Split('+')[1];
                isPlus = true;
            } else if (addStatements[1].Contains("-"))
            {
                field = addStatements[1].Split('-')[0];
                addValue = addStatements[1].Split('-')[1];
            }

            var indexKey = Array.FindIndex(csvTable.Columns,
                row => row.ToString() == field);
            var resultKey = csvRow[indexKey];

            var resultDatetime = new DateTime();
            resultDatetime = DateTime.Parse(resultKey);

            if (!addValue.Contains("MONTH")) return result;

            var newResult = addValue.Replace("MONTH", "");
            resultDatetime = isPlus ? resultDatetime.AddMonths(Parse(newResult)) : resultDatetime.AddMonths(Parse(newResult) * -1);

            return resultDatetime.ToString();
        }


        public string GetResultFromKey(string currentResult, MappingRow mappingRow, CsvTable csvTable, string[] csvRow, Token token)
        {
            var result = currentResult;
            if (!mappingRow.Remark.Contains("KEY")) return result;

            var stringSeparators = new[] {"KEY:"};
            var keyStatements = mappingRow.Remark.Split(stringSeparators, StringSplitOptions.None)[1];
            var keys = keyStatements.Split('=');
            var primaryKeyLocation = keys[0];
            var splits = keys[1].Split('&');
            var foreignKey = splits[0];
            var primaryKey = splits[1];

            var indexForeignKey = Array.FindIndex(csvTable.Columns,
                row => row.ToString() == foreignKey);
            var resultForeignKey = csvRow[indexForeignKey];

            var requestService = new RequestService();
            result = requestService.GetRequest(primaryKeyLocation.ToLower(),
                @"[" + primaryKey + "]='" + resultForeignKey + "'", null, keys[2]);

            return result;
        }

        public string GetResultFromCustomIf(string currentResult, MappingRow mappingRow, CsvTable csvTable, string[] csvRow, Token token)
        {
            var result = currentResult;

            if (!mappingRow.Remark.Contains("IF")) return result;
            var stringSeparators = new[] {"IF:"};
            var ifStatements = mappingRow.Remark.Split(stringSeparators, StringSplitOptions.None);
            foreach (var ifStatement in ifStatements)
            {
                if (!ifStatement.Contains("=")) continue;

                var statement = ifStatement.Replace(",", "").Split('=')[0];
                var newValue = ifStatement.Replace(",", "").Split('=')[1];
                if (result.ToUpper() == statement)
                    result = newValue.Replace(" ", "");
            }

            return result;
        }

        public string GetResultFromMerge(string currentResult, MappingRow mappingRow, CsvTable csvTable, string[] csvRow, Token token)
        {
            var result = currentResult;

            if (!mappingRow.Remark.Contains("MERGE")) return result;

            result = "";
            var stringSeparators = new[] { "MERGE=" };
            var mergeStatements =
                mappingRow.Remark.Split(stringSeparators, StringSplitOptions.None)[1];
            var merges = mergeStatements.Split('&');
            foreach (var merge in merges)
            {
                var indexMerge = Array.FindIndex(csvTable.Columns,
                    row => row.ToString() == merge);
                if (string.IsNullOrEmpty(result))
                    result = csvRow[indexMerge];
                else
                    result = result + " " + csvRow[indexMerge];
            }

            return result;
        }
    }
}