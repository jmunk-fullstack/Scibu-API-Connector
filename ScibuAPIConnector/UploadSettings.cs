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
        public static string[] UploadFiles { get; set; }
        public static string DatabaseName { get; set; }
        public static string DatabaseUsername { get; set; }
        public static string DatabasePassword { get; set; }
        public static string ClientSecret { get; set; }
        public static Token Token { get; set; }
        
        public static JArray Companies { get; set; }
        public static JArray Invoices { get; set; }
        public static JArray Quotes { get; set; }
        public static JArray Orders { get; set; }

    }
}
