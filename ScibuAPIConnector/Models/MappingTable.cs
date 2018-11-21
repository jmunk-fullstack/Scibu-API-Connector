using System.Collections.Generic;

namespace ScibuAPIConnector.Models
{
    public class MappingTable
    {
        public string CsvName { get; set; }
        public List<MappingRow> Rows { get; set; }
    }

    public class MappingRow
    {
        public string FieldInCsv { get; set; }
        public string CsvName { get; set; }
        public string ApiCall { get; set; }
        public string ApiField { get; set; }
        public string DefaultValue { get; set; }
        public string Remark { get; set; }
    }
}
