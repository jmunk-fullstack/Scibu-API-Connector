namespace ScibuAPIConnector
{
    using ScibuAPIConnector.Services;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Timers;

    internal static class Program
    {
        public static void CreateIfMissing(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    DirectoryInfo info = Directory.CreateDirectory(path);
                }
            }
            catch (IOException exception1)
            {
                Console.WriteLine(exception1.Message);
            }
        }

        private static void Main(string[] args)
        {
            RunApp();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Got a new access token!");
            UploadSettings.Token = new AuthorizationService().GetToken(UploadSettings.DatabaseUsername, UploadSettings.DatabasePassword, UploadSettings.DatabaseName, UploadSettings.ClientSecret);
        }

        public static void ReadConfig()
        {
            UploadSettings.DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
            UploadSettings.DatabasePassword = ConfigurationManager.AppSettings["DatabasePassword"];
            UploadSettings.DatabaseUsername = ConfigurationManager.AppSettings["DatabaseUsername"];
            UploadSettings.ClientSecret = ConfigurationManager.AppSettings["ClientSecret"];
            UploadSettings.Token = new AuthorizationService().GetToken(UploadSettings.DatabaseUsername, UploadSettings.DatabasePassword, UploadSettings.DatabaseName, UploadSettings.ClientSecret);
            char[] separator = new char[] { ',' };
            UploadSettings.UploadFiles = ConfigurationManager.AppSettings["UploadFiles"].Split(separator);
            UploadSettings.UploadName = ConfigurationManager.AppSettings["UploadName"];
            UploadSettings.UploadType = ConfigurationManager.AppSettings["UploadType"];
            UploadSettings.UploadSeperator = ConfigurationManager.AppSettings["UploadSeperator"];
            UploadSettings.MappingFileLocation = ConfigurationManager.AppSettings["MappingFileLocation"];
            UploadSettings.CronjobInMinutes = ConfigurationManager.AppSettings["CronjobInMinutes"];
            UploadSettings.ImportLocation = ConfigurationManager.AppSettings["ImportLocation"];
            UploadSettings.DownloadFiles = ConfigurationManager.AppSettings["DownloadFiles"];
            UploadSettings.FilesUrl = ConfigurationManager.AppSettings["FilesUrl"];
            UploadSettings.MailFrom = ConfigurationManager.AppSettings["MailFrom"];
            UploadSettings.MailTo = ConfigurationManager.AppSettings["MailTo"];
            UploadSettings.LastDateUpdate = ConfigurationManager.AppSettings["LastDateUpdate"];
            UploadSettings.SaveDates = ConfigurationManager.AppSettings["SaveDates"];
            UploadSettings.AllDirectories = ConfigurationManager.AppSettings["AllDirectories"];
            UploadSettings.DeleteFTPFiles = ConfigurationManager.AppSettings["DeleteFTPFiles"];
            UploadSettings.AttachmentUrl = ConfigurationManager.AppSettings["AttachmentUrl"];
            if (UploadSettings.AllDirectories == "true")
            {
                UploadSettings.SaveDates = "false";
            }
            if (UploadSettings.SaveDates == "true")
            {
                UploadSettings.ImportLocation = UploadSettings.ImportLocation + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\";
                CreateIfMissing(UploadSettings.ImportLocation);
            }
        }

        public static void RunApp()
        {
            Console.WriteLine("Starting the application...");
            Console.WriteLine("Reading config file...");
            ReadConfig();
            Console.WriteLine("Done reading the config file!");
            Console.WriteLine("Downloading FTP files");
            FtpService ftpService = new FtpService();
            ftpService.DownloadFiles();
            Console.WriteLine("Done downloading the FTP files");
            List<string> list = new List<string>();
            if (UploadSettings.AllDirectories != "true")
            {
                StartSync();
            }
            else
            {
                foreach (string str in Directory.GetDirectories(UploadSettings.ImportLocation).ToList<string>())
                {
                    Console.WriteLine("Starting to import directory " + str);
                    UploadSettings.ImportLocation = str + @"\";
                    StartSync();
                }
            }
            Environment.Exit(0);
        }

        public static void StartSync()
        {
            Console.WriteLine("Found " + UploadSettings.UploadType + " files: ");
            string str = UploadSettings.UploadFiles.Last<string>();
            foreach (string str2 in UploadSettings.UploadFiles)
            {
                if (str2.Equals(str))
                {
                    Console.Write(str2);
                }
                else
                {
                    Console.Write(str2 + ", ");
                }
            }
            Console.WriteLine("");
            Timer timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(Program.OnTimedEvent);
            timer.Interval = 2700000.0;
            timer.Enabled = true;
            CacheService.CacheCompanies();
            CacheService.CacheContacts();
            CacheService.CacheQuotes();
            CacheService.CacheQuoteDiscount();
            CacheService.CacheQuoteProducts();
            CacheService.CacheTickets();
            CacheService.CacheInvoices();
            CacheService.CacheOrders();
            CacheService.CacheOrderProducts();
            CacheService.CacheOrderDiscount();
            CacheService.CacheInvoiceProducts();
            new MappingService().GenerateMapping();
            Console.WriteLine("Mapping is done and everything is send to the API!");
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration((ConfigurationUserLevel)ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["LastDateUpdate"].Value = DateTime.Now.Date.ToShortDateString();
            configuration.Save((ConfigurationSaveMode)ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("appSettings");
            CacheService.Dispose();
        }
    }
}

