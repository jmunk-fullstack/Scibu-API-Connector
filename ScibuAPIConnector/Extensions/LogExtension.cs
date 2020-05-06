namespace ScibuAPIConnector.Extensions
{
    using ScibuAPIConnector;
    using System;
    using System.Configuration;
    using System.Data.SqlClient;

    public static class LogExtension
    {
        public static void Log(string exception, DateTime logDate, string input, string output)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["GlobalConnection"].ConnectionString))
            {
                connection.Open();
                try
                {
                    if (input.Length > 0x1387)
                    {
                        input = input.Substring(0, 0x1387);
                    }
                    if (output.Length > 0x1387)
                    {
                        output = output.Substring(0, 0x1387);
                    }
                    if (output.Length > 0x1387)
                    {
                        exception = exception.Substring(0, 0x1387);
                    }
                    string[] textArray1 = new string[9];
                    textArray1[0] = "INSERT INTO API_CONNECTOR_LOG\r\n                                    (DATABASE_NAME, LOG_DATE, EXCEPTION, INPUT, OUTPUT)\r\n                                    VALUES('";
                    textArray1[1] = UploadSettings.DatabaseName;
                    textArray1[2] = "',  GETDATE(), '";
                    textArray1[3] = exception;
                    textArray1[4] = "', '";
                    textArray1[5] = input;
                    textArray1[6] = "', '";
                    textArray1[7] = output;
                    textArray1[8] = "')";
                    new SqlCommand(string.Concat(textArray1), connection).ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception)
                {
                    new SqlCommand("INSERT INTO API_CONNECTOR_LOG\r\n                                    (DATABASE_NAME, LOG_DATE, EXCEPTION, INPUT, OUTPUT)\r\n                                    VALUES('" + UploadSettings.DatabaseName + "',  GETDATE(), 'Message too big', 'Input too big', 'Output too big')", connection).ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
    }
}

