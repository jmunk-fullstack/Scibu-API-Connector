using System;
using System.Collections.Generic;
using System.Linq;
using ScibuAPIConnector.Extensions;
using ScibuAPIConnector.Models;

namespace ScibuAPIConnector.Services
{
    public class MappingService
    {
        public CsvTable MapCsv(string csvName, string csvFile)
        {
            //Get all the lines from the CSV file
            var csvReader = new CsvReader();
            var getCsvLines = csvReader.ReadCsv(csvFile);

            //First line is the column
            var columns = getCsvLines[0].Split('\t').ToArray();

            //Remove special characters
            for (var i = 0; i < columns.Length; i++)
            {
                columns[i] = columns[i].RemoveSpecialCharacters();
            }

            //Creating the row
            var rows = new List<string[]>();

            var count = 0;
            foreach (var row in getCsvLines)
            {
                //Count 0 is the column
                if (count != 0)
                {
                    //The CSV needs to be tab-delimited
                    var csvRow = row.Split('\t').ToArray();

                    for (var i = 0; i < csvRow.Length; i++)
                    {
                        csvRow[i] = csvRow[i].RemoveSpecialCharacters();
                    }

                    rows.Add(csvRow);
                }

                count++;
            }

            return new CsvTable(csvName, columns, rows);
        }

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

        public List<List<MappingRow>> GetUniqueMappingRows(List<CsvTable> csvTables)
        {
            //Get the Mapping table
            var mappingTables = MapXls(@"\\192.168.0.37\Klanten\HKV Ochten\mapping\Testing_Mapping_Jordan.xlsx");

            var uniqueLists = new List<List<List<MappingRow>>>();

            foreach (var csvTable in csvTables)
            {
                // Get all API calls in unique list
                var uniqueMappingRows = new List<List<MappingRow>>();

                //Loop through the mapping tables
                foreach (var mappingTable in mappingTables)
                {
                    //Check if the CSV name is equal to the mapping csv table.
                    if (mappingTable.CsvName != csvTable.CsvName) continue;

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
            //Get unique mapping rows by API call
            var csvTables = UploadSettings.UploadFiles
                .Select(file => MapCsv(file, @"\\192.168.0.37\Klanten\HKV Ochten\Import\Testcase\" + file + ".csv")).ToList();
            var uniqueMappingRows = GetUniqueMappingRows(csvTables);

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
                var tables = new List<CsvTable>();
                // Add API Function
                foreach (var csvTable in csvTables)
                {
                    if (csvTable.CsvName != checkMap[0].CsvName) continue;
                    tables.Add(csvTable);

                    if (csvTable.CsvName != "Klanten") continue;
                    foreach (var csvTable2 in csvTables)
                    {
                        if (csvTable2.CsvName == "Klant_adressen")
                        {
                            tables.Add(csvTable2);
                        }
                    }
                }

                apiService.AddToAPi(checkMap, tables);
            }
        }
    }
}