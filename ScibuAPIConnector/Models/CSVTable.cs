using System.Collections.Generic;

namespace ScibuAPIConnector.Models
{
    public class CsvTable
    {
        public string CsvName { get; set; }
        public string[] Columns { get; set; }
        public List<string[]> Rows { get; set; }

        public CsvTable(string csvName, string[] columns, List<string[]> rows)
        {
            CsvName = csvName;
            Columns = columns;
            Rows = rows;
        }
    }
}