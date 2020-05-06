namespace ScibuAPIConnector.Services
{
    using ScibuAPIConnector.Extensions;
    using ScibuAPIConnector.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class CsvReader
    {
        public ImportTable MapCsv(string csvName, string csvFile)
        {
            List<string> list = this.ReadCsv(csvFile);
            char[] separator = new char[] { '\t' };
            string[] columns = list[0].Split(separator).ToArray<string>();
            int index = 0;
            while (true)
            {
                if (index >= columns.Length)
                {
                    List<string[]> rows = new List<string[]>();
                    int num = 0;
                    foreach (string str in list)
                    {
                        if (num != 0)
                        {
                            char[] chArray2 = new char[] { '\t' };
                            string[] item = str.Split(chArray2).ToArray<string>();
                            int num3 = 0;
                            while (true)
                            {
                                if (num3 >= item.Length)
                                {
                                    rows.Add(item);
                                    break;
                                }
                                item[num3] = item[num3].HtmlDecode().RemoveSpecialCharacters();
                                num3++;
                            }
                        }
                        num++;
                    }
                    return new ImportTable(csvName, columns, rows);
                }
                columns[index] = columns[index].RemoveSpecialCharacters();
                index++;
            }
        }

        public List<string> ReadCsv(string fileName)
        {
            List<string> list3;
            using (StreamReader reader = new StreamReader(fileName))
            {
                List<string> list = new List<string>();
                List<string> list2 = new List<string>();
                while (true)
                {
                    if (reader.EndOfStream)
                    {
                        list3 = list;
                        break;
                    }
                    string item = reader.ReadLine();
                    char[] separator = new char[] { ';' };
                    string[] strArray = item.Split(separator);
                    list.Add(item);
                }
            }
            return list3;
        }
    }
}

