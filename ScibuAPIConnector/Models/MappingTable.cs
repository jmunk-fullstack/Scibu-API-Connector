namespace ScibuAPIConnector.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class MappingTable
    {
        public string CsvName { get; set; }

        public List<MappingRow> Rows { get; set; }
    }
}

