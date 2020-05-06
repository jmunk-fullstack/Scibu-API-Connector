namespace ScibuAPIConnector.Models
{
    using System;
    using System.Runtime.CompilerServices;

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

