using System.Collections.Generic;
using System.IO;

namespace ScibuAPIConnector.Services
{
    public class CsvReader
    {
        public List<string> ReadCsv(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                var listA = new List<string>();
                var listB = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    listA.Add(values[0]);
              //      listB.Add(values[1]);
                }

                return listA;
            }
        }
    }
}
