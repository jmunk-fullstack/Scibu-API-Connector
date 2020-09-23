using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ScibuAPIConnector.Models;
using ScibuAPIConnector.Services;

namespace ScibuAPIConnector
{
    public static class UploadSettings
    {
        public static string UploadName { get; set; }
        public static string UploadType { get; set; }
        public static string UploadCall { get; set; }
        public static string UploadSeperator { get; set; }
        public static string[] UploadFiles { get; set; }
        public static string MappingFileLocation { get; set; }
        public static string FilesUrl { get; set; }
        public static string DatabaseName { get; set; }
        public static string DatabaseUsername { get; set; }
        public static string DatabasePassword { get; set; }
        public static string ClientSecret { get; set; }
        public static string CronjobInMinutes { get; set; }
        public static string ImportLocation { get; set; }
        public static string DownloadFiles { get; set; }
        public static string SaveDates { get; set; }
        public static string LastDateUpdate { get; set; }
        public static string MailTo { get; set; }
        public static string MailFrom { get; set; }
        public static string AllDirectories { get; set; }
        public static string DeleteFTPFiles { get; set; }
        public static string AttachmentUrl { get; set; }
        public static Token Token { get; set; }
        
        public static JArray Companies { get; set; }
        public static JArray Invoices { get; set; }
        public static JArray Quotes { get; set; }
        public static JArray QuoteProducts { get; set; }
        public static JArray QuoteDiscounts { get; set; }
        public static JArray OrderDiscounts { get; set; }
        public static JArray Orders { get; set; }
        public static JArray Contacts { get; set; }
        public static JArray OrderProducts { get; set; }
        public static JArray Tickets { get; set; }
        public static JArray InvoiceProducts { get; set; }

    }
}
