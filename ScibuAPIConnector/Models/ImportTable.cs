namespace ScibuAPIConnector.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ImportTable
    {
        public ImportTable(string importName, string[] columns, List<string[]> rows)
        {
            this.ImportName = importName;
            this.Columns = columns;
            this.Rows = rows;
        }

        public string ImportName { get; set; }

        public string[] Columns { get; set; }

        public List<string[]> Rows { get; set; }
    }
}

