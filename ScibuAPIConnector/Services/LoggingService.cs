using System;
using System.Data.SqlClient;

namespace ScibuAPIConnector.Services
{
    public static class LoggingService
    {
        public static void Logging(string databaseName, string uploadType, string uploadName, string remark, string remarkType, string uploadCall)
        {
            using (var connection = DatabaseService.GetConnection())
            {
                try
                {
                    connection.Open();
                    
                    var query = @"INSERT INTO API_MAPPER_LOG
                                    (DATABASE_NAME, UPLOAD_TYPE, UPLOAD_NAME, REMARK, REMARK_TYPE, UPLOAD_CALL, LOG_DATE)
                                    VALUES('" + databaseName + "', '" + uploadType + "', '" + uploadName + "', '" + remark + "', '" + remarkType + "', '"+ uploadCall + "', GETDATE())";


                    var command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();

                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw new System.InvalidOperationException("Can't upload log to database. Error: " + ex);
                }

            }
        }
    }
}
