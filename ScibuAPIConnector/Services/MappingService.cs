using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ScibuAPIConnector.Extensions;
using ScibuAPIConnector.Models;

namespace ScibuAPIConnector.Services
{
    public class MappingService
    {
        

        public List<MappingTable> MapXls(string xlsFile)
        {
            //Get all data from XLS and create an empty list of mapping tables.
            var excelReader = new ExcelReader();
            var table = excelReader.ReadExcel(xlsFile);
            var mappingTables = new List<MappingTable>();

            //CurrentMapping -1 because an array starts at [0]. And when it get found it will +1.
            var currentMapping = -1;

            //Counting Rows
            for (var r = 0; r < table.Rows.Count; r++)
            {
                //Creating a row
                var mappingRow = new MappingRow();

                //Counting Columns
                for (var c = 0; c < table.Columns.Count; c++)
                {
                    var currentRow = table.Rows[r][c];

                    //Making a new mapping table when the string contains "Mapping bestand". 
                    if (currentRow.ToString().Contains("Mapping bestand"))
                    {
                        currentMapping++;
                        var mappingTable = new MappingTable
                        {
                            CsvName = currentRow.ToString()
                                .Split(new[] { "Mapping bestand " }, StringSplitOptions.None)[1]
                        };
                        mappingTables.Add(mappingTable);
                    }
                    else
                    {
                        switch (c)
                        {
                            //Get all the data for the mappingRow
                            case 0:
                                mappingRow.FieldInCsv = currentRow.ToString();
                                break;
                            case 1:
                                mappingRow.ApiCall = currentRow.ToString();
                                break;
                            case 2:
                                mappingRow.ApiField = currentRow.ToString();
                                break;
                            case 3:
                                mappingRow.DefaultValue = currentRow.ToString();
                                break;
                            case 4:
                                mappingRow.Remark = currentRow.ToString();
                                break;
                        }
                    }

                    mappingRow.CsvName = mappingTables[currentMapping].CsvName;
                }

                //When there is no row, it will generate one
                if (mappingTables[currentMapping].Rows == null)
                {
                    var mappingRows = new List<MappingRow>();
                    mappingTables[currentMapping].Rows = mappingRows;
                }

                //Checks if the row is not all empty and is not the header else it will add a new mappingrow to the current mappingtable.
                if (!mappingRow.FieldInCsv.HasContent() && !mappingRow.ApiCall.HasContent() &&
                    !mappingRow.ApiField.HasContent() && !mappingRow.DefaultValue.HasContent() &&
                    !mappingRow.Remark.HasContent()) continue;

                if (mappingRow.FieldInCsv != "Veld in bestand" && mappingRow.ApiCall != "API Call" &&
                    mappingRow.ApiField != "API Veld" && mappingRow.DefaultValue != "Standaard Waarde" &&
                    mappingRow.Remark != "Opmerking")
                {
                    mappingTables[currentMapping].Rows.Add(mappingRow);
                }
            }

            return mappingTables;
        }

        public List<List<MappingRow>> GetUniqueMappingRows(List<ImportTable> csvTables)
        {
            //Get the Mapping table
            var mappingTables = MapXls(UploadSettings.MappingFileLocation);

            var uniqueLists = new List<List<List<MappingRow>>>();

            foreach (var csvTable in csvTables)
            {
                // Get all API calls in unique list
                var uniqueMappingRows = new List<List<MappingRow>>();

                //Loop through the mapping tables
                foreach (var mappingTable in mappingTables)
                {
                    //Check if the CSV name is equal to the mapping csv table.
                    if (mappingTable.CsvName != csvTable.ImportName) continue;

                    //Loop through all rows
                    foreach (var mappingRow in mappingTable.Rows)
                    {
                        //If there are zero rows in the unique mapping rows, then add the first.
                        if (uniqueMappingRows.Count == 0)
                        {
                            var list = new List<MappingRow>
                            {
                                mappingRow
                            };

                            uniqueMappingRows.Add(list);
                        }
                        else
                        {
                            //If there is a duplicate it will add the mappingRow to the current in the loop, else it will make a new one.
                            var duplicate = false;

                            //Loop through the list in the list
                            foreach (var unique in uniqueMappingRows.ToList())
                            {
                                foreach (var uniqueRow in unique.ToList())
                                {
                                    //If there is a duplicate, go to the next list.
                                    if (duplicate) continue;

                                    //If the API calls are equal then add it to the same row.
                                    if (mappingRow.ApiCall != uniqueRow.ApiCall) continue;

                                    unique.Add(mappingRow);
                                    duplicate = true;
                                }
                            }

                            //Add the new one if there were no duplicates
                            if (duplicate) continue;
                            var list = new List<MappingRow>
                            {
                                mappingRow
                            };

                            uniqueMappingRows.Add(list);
                        }
                    }
                }

                uniqueLists.Add(uniqueMappingRows);
            }

            //Merge lists
            var newList = new List<List<MappingRow>>();
            foreach (var lists in uniqueLists)
            {
                foreach (var list in lists)
                {
                    newList.Add(list);
                }
            }

            return newList;
        }

        public void GenerateMapping()
        {
            List<ImportTable> importTables = new List<ImportTable>();
            if ((UploadSettings.UploadFiles.Length != 1) || (UploadSettings.UploadFiles[0] != ""))
            {
                //Get unique mapping rows by API call
                if (UploadSettings.UploadType == "CSV")
                {
                    CsvReader CsvReader = new CsvReader();
                    importTables = (from file in UploadSettings.UploadFiles select CsvReader.MapCsv(file, UploadSettings.ImportLocation + file + ".csv", UploadSettings.UploadSeperator)).ToList<ImportTable>();
                }
                if (UploadSettings.UploadType == "XML")
                {
                    XmlReader XmlReader = new XmlReader();
                    importTables = (from file in UploadSettings.UploadFiles select XmlReader.MapXml(file, UploadSettings.ImportLocation + file + ".xml")).ToList<ImportTable>();
                }
                if (UploadSettings.UploadType == "TECHNEA_XML")
                {
                    TechneaXMLReader techneaXMLReader = new TechneaXMLReader();
                    importTables = techneaXMLReader.GetImportTables(UploadSettings.ImportLocation);
                }
                if (UploadSettings.UploadType == "UBL")
                {
                    UblReader UblReader = new UblReader();
                    importTables = UblReader.GetImportTables(UploadSettings.ImportLocation + "Facturen.xml");
                }

                var uniqueMappingRows = GetUniqueMappingRows(importTables);

                //Merge customs
                foreach (var checkMap in uniqueMappingRows.ToList())
                {
                    //Get customs
                    var apiService = new ApiService();
                    var customs = apiService.GetCustoms();

                    foreach (var custom in customs)
                    {
                        if (!checkMap[0].ApiCall.ToUpper().Contains(custom) && !checkMap[0].Remark.ToUpper().Contains(custom)) continue;

                        // Merging custom fields with original
                        if (custom.Equals("CUSTOMFIELD"))
                        {
                            // Add Custom Field Function
                            apiService.AddCustomField(checkMap);

                            //Get the API Call
                            var mapWithoutCustom = checkMap[0].ApiCall.Replace("customfield", "");

                            //Combine the customfield list to the normal one
                            foreach (var checkMap2 in uniqueMappingRows.ToList())
                            {
                                if (checkMap2[0].ApiCall != mapWithoutCustom) continue;

                                checkMap2.AddRange(checkMap);
                                uniqueMappingRows.Remove(checkMap);
                            }

                            //Remove customfield to the API Calls
                            foreach (var map in checkMap.ToList())
                            {
                                map.ApiCall = mapWithoutCustom;
                                map.Remark = "CUSTOMFIELD";
                            }
                        }
                        else
                        {
                            var mapTo = "";
                            if (checkMap[0].Remark.Contains("MAPTO_INSIDE"))
                            {
                                mapTo = checkMap[0].Remark.Split('=')[1];
                                checkMap[0].ApiField = mapTo + "_" + custom + "_" + checkMap[0].ApiField;
                            }

                            //Combine the customfield list to the mapper
                            foreach (var checkMap2 in uniqueMappingRows.ToList())
                            {
                                if (mapTo == "") continue;
                                if (!checkMap2[0].ApiCall.ToUpper().Contains(mapTo)) continue;

                                checkMap2.AddRange(checkMap);
                                uniqueMappingRows.Remove(checkMap);
                            }
                        }
                    }
                }

                foreach (var checkMap in uniqueMappingRows)
                {
                    if (checkMap[0].ApiCall.Contains("customfield")) continue;

                    // Check if 
                    var apiService = new ApiService();
                    var tables = new List<ImportTable>();
                    // Add API Function
                    foreach (var table in importTables)
                    {
                        if (table.ImportName != checkMap[0].CsvName) continue;
                        tables.Add(table);

                        if (table.ImportName != "Klanten") continue;
                        foreach (var table2 in importTables)
                        {
                            if (table2.ImportName == "Klant_adressen")
                            {
                                tables.Add(table2);
                            }
                        }
                    }

                    if (tables[0].Rows.Count >= 10000000)
                    {
                        var tableCount = tables[0].Rows.Count;
                        var oldTable = tables[0].Rows;
                        for (var i = 0; i < tableCount; i += 1000)
                        {
                            tables[0].Rows = oldTable.Skip(i).Take(1000).ToList();
                            apiService.AddToAPi(checkMap, tables);
                        }
                    }
                    else
                    {
                        apiService.AddToAPi(checkMap, tables);
                    }

                    Console.WriteLine("Done adding " + checkMap[0].CsvName);
                }
            }
        }
    }
}