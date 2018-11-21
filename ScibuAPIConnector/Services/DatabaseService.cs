using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using ScibuAPIConnector.Models;

namespace ScibuAPIConnector.Services
{
    public class DatabaseService
    {
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["GlobalConnection"].ConnectionString);
        }

        public static Dictionary<string, Client> GetAllClients()
        {
            var allClients = new Dictionary<string, Client>();

            using (var myConnection = GetConnection())
            {
                var sql = @"SELECT *
                        FROM api_client";

                var oCmd = new SqlCommand(sql, myConnection);
                myConnection.Open();
                using (var oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        var client = new Client
                        {
                            ClientId = oReader["client_id"].ToString().TrimEnd(),
                            Secret = oReader["client_secret"].ToString().TrimEnd(),
                            Name = oReader["client_name"].ToString().TrimEnd(),
                            Active = (bool) oReader["active"],
                            DatabaseName = oReader["database_name"].ToString().TrimEnd()
                        };

                        allClients.Add(client.ClientId, client);
                    }

                    myConnection.Close();
                }
            }
            return allClients;
        }
    }
}
