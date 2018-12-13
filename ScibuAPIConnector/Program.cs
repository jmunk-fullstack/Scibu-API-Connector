using ScibuAPIConnector.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScibuAPIConnector
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("Starting the application...");
            UploadSettings.DatabaseName = "hkvportal";
            UploadSettings.DatabasePassword = "MCAw87m8xXGfhnqC";
            UploadSettings.DatabaseUsername = "scibuadmin";
         //   UploadSettings.DatabaseName = "testportal";
           // UploadSettings.DatabasePassword = "Welkom0001";
            //UploadSettings.DatabaseUsername = "jordan";
            UploadSettings.ClientSecret = "secret123";
            UploadSettings.Token = new AuthorizationService().GetToken(UploadSettings.DatabaseUsername,
                UploadSettings.DatabasePassword, UploadSettings.DatabaseName, UploadSettings.ClientSecret);
            UploadSettings.UploadFiles = new[]{"Orderregels"};
           // UploadSettings.UploadFiles = new[] { "Contactpersonen" };
            UploadSettings.UploadName = "Klanten";
            UploadSettings.UploadType = "CSV";
            
            Console.WriteLine("Found CSV files: ");
            var last = UploadSettings.UploadFiles.Last();
            foreach (var file in UploadSettings.UploadFiles)
            {
                if(file.Equals(last))
                    Console.Write(file);
                else
                    Console.Write(file + ", ");
            }
            Console.WriteLine("");

            var aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 2700000;
            aTimer.Enabled = true;

            CacheService.CacheCompanies();
            CacheService.CacheQuotes();
            CacheService.CacheInvoices();
            CacheService.CacheOrders();

           // UploadSettings.Companies = new JArray();

            var mappingService = new MappingService();
            mappingService.GenerateMapping();
            Console.WriteLine("Mapping is done and everything is send to the API!");
            Console.ReadKey();
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Got a new access token!");

            UploadSettings.Token = new AuthorizationService().GetToken(UploadSettings.DatabaseUsername,
                UploadSettings.DatabasePassword, UploadSettings.DatabaseName, UploadSettings.ClientSecret);
        }
    }
}
