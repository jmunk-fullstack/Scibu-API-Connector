using ExcelDataReader;
using System.Data;
using System.IO;

namespace ScibuAPIConnector.Services
{
    public class ExcelReader
    {
        public DataTable ReadExcel(string filePath)
        {
            var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader;

            //1. Reading Excel file
            excelReader = Path.GetExtension(filePath).ToUpper() == ".XLS" ? ExcelReaderFactory.CreateBinaryReader(stream) : ExcelReaderFactory.CreateOpenXmlReader(stream);

            //2. DataSet - The result of each spreadsheet will be created in the result.Tables
            var result = excelReader.AsDataSet();
            return result.Tables[0];
        }
    }
}
