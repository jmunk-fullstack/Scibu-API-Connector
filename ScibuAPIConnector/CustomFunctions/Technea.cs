namespace ScibuAPIConnector.CustomFunctions
{
    using Newtonsoft.Json;
    using ScibuAPIConnector.Extensions;
    using ScibuAPIConnector.Models;
    using ScibuAPIConnector.Services;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;

    public static class Technea
    {
        public static bool TypeCheck(string result, string[] csvRow, string[] columns, List<MappingRow> mappingRows, Token token)
        {
            bool flag24;
            if (result.ToLower() != "p")
            {
                flag24 = false;
            }
            else
            {
                int index = -1;
                Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
                dictionary1.Add("Dear", "heer/mevrouw");
                dictionary1.Add("Tav", "heer/mevrouw");
                dictionary1.Add("JobTitle", "Beste heer/mevrouw");
                dictionary1.Add("Initial", "");
                dictionary1.Add("Tag", "");
                dictionary1.Add("GMAccountNo", "");
                dictionary1.Add("VATnumber", "");
                dictionary1.Add("Status", "Klant");
                dictionary1.Add("Source", "");
                dictionary1.Add("PriceProfileId", "1");
                dictionary1.Add("AccountManager", "");
                dictionary1.Add("Inactive", "false");
                dictionary1.Add("SpeakLanguage", "Nederlands");
                dictionary1.Add("Language", "NL");
                dictionary1.Add("Gender", "Onbekend");
                dictionary1.Add("NewsLetter", "false");
                dictionary1.Add("Department", "");
                Dictionary<string, object> dictionary = dictionary1;
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                Dictionary<string, object> item = new Dictionary<string, object>();
                Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
                bool flag2 = false;
                bool flag3 = false;
                foreach (MappingRow row in mappingRows)
                {
                    string search = row.FieldInCsv;
                    if (row.FieldInCsv != "")
                    {
                        index = Array.FindIndex<string>(columns, rowTechnea => rowTechnea.ToString() == search);
                        string str = result = csvRow[index];
                        if (row.FieldInCsv.ToLower() == "relatienaam")
                        {
                            char[] separator = new char[] { ' ' };
                            string[] strArray = str.Split(separator);
                            char[] chArray2 = new char[] { ' ' };
                            dictionary.Add("FirstName", str.Split(chArray2)[0]);
                            if (strArray.Length == 3)
                            {
                                char[] chArray3 = new char[] { ' ' };
                                char[] chArray4 = new char[] { ' ' };
                                dictionary.Add("LastName", str.Split(chArray3)[1] + " " + str.Split(chArray4)[2]);
                            }
                            else if (!str.Contains(" "))
                            {
                                dictionary.Add("LastName", "");
                            }
                            else
                            {
                                char[] chArray5 = new char[] { ' ' };
                                dictionary.Add("LastName", str.Split(chArray5)[1]);
                            }
                        }
                        if (row.FieldInCsv.ToLower() == "relatienummer")
                        {
                            dictionary.Add("ExternalId", str);
                            dictionary.Add("CustomerNumber", str);
                        }
                        if (row.FieldInCsv.ToLower() == "telefoonnummer")
                        {
                            List<Dictionary<string, object>> list2 = new List<Dictionary<string, object>>();
                            Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
                            dictionary6.Add("PhoneNumber", str);
                            dictionary6.Add("PhoneType", "Algemeen");
                            dictionary6.Add("WorkOrHome", "Zakelijk");
                            list2.Add(dictionary6);
                            dictionary.Add("Phone", list2);
                        }
                        if ((row.FieldInCsv.ToLower() == "bezoekadres") && !flag2)
                        {
                            item.Add("Street", StringExtensions.ReplaceNumbers(str));
                            item.Add("HouseNr", StringExtensions.GetNumbers(str));
                            item.Add("Latitude", "0");
                            item.Add("Longitude", "0");
                            item.Add("AddressType", "Bezoek");
                            flag2 = true;
                        }
                        if (row.FieldInCsv.ToLower() == "bezoekpostcode")
                        {
                            item.Add("Postal", str);
                        }
                        if (row.FieldInCsv.ToLower() == "bezoekplaats")
                        {
                            item.Add("City", str);
                        }
                        if (row.FieldInCsv.ToLower() == "bezoekland")
                        {
                            item.Add("Country", str);
                        }
                        if ((row.FieldInCsv.ToLower() == "postadres") && !flag3)
                        {
                            dictionary3.Add("Street", StringExtensions.ReplaceNumbers(str));
                            dictionary3.Add("HouseNr", StringExtensions.GetNumbers(str));
                            dictionary3.Add("Latitude", "0");
                            dictionary3.Add("Longitude", "0");
                            dictionary3.Add("AddressType", "Bezoek");
                            flag3 = true;
                        }
                        if (row.FieldInCsv.ToLower() == "postpostcode")
                        {
                            dictionary3.Add("Postal", str);
                        }
                        if (row.FieldInCsv.ToLower() == "postplaats")
                        {
                            dictionary3.Add("City", str);
                        }
                        if (row.FieldInCsv.ToLower() == "postland")
                        {
                            dictionary3.Add("Country", str);
                        }
                        if ((row.FieldInCsv.ToLower() == "email") && (str != ""))
                        {
                            List<Dictionary<string, object>> list3 = new List<Dictionary<string, object>>();
                            Dictionary<string, object> dictionary7 = new Dictionary<string, object>();
                            dictionary7.Add("Mail", str);
                            dictionary7.Add("WorkOrHome", "Zakelijk");
                            list3.Add(dictionary7);
                            dictionary.Add("Email", list3);
                        }
                    }
                }
                foreach (KeyValuePair<string, object> pair in item)
                {
                    bool flag20 = pair.Key == "Street";
                    if (flag20 && ((pair.Value != null) && (pair.Value.ToString() != "")))
                    {
                        list.Add(item);
                    }
                }
                foreach (KeyValuePair<string, object> pair2 in dictionary3)
                {
                    bool flag22 = pair2.Key == "Street";
                    if (flag22 && ((pair2.Value != null) && (pair2.Value.ToString() != "")))
                    {
                        list.Add(item);
                    }
                }
                dictionary.Add("Address", list);
                new RequestService().PostRequest("contact", JsonConvert.SerializeObject(dictionary), 0);
                flag24 = true;
            }
            return flag24;
        }

        public static void ImportPDF(string invoiceId, string invoiceName)
        {
            var pdfFile = AppDomain.CurrentDomain.BaseDirectory + "/Import Files/SI" + invoiceName + ".pdf";
            var attachmentLocation = UploadSettings.AttachmentUrl + invoiceId;

            Console.WriteLine("Moving pdf file" );
            Console.WriteLine("From: " + pdfFile);
            Console.WriteLine("To: " + attachmentLocation + "\\SI" + invoiceName + ".pdf");
            //attachmentLocation = @"E:\Qubics\Api Connector\Technea2\Import Files\" + invoiceId;
            try
            {
                if (File.Exists(pdfFile))
                {
                    Console.WriteLine("PDF File exist");
                    if (File.Exists(attachmentLocation + "\\SI" + invoiceName + ".pdf"))
                    {
                        File.Delete(attachmentLocation + "\\SI" + invoiceName + ".pdf");
                    }
                    Console.WriteLine("Moving PDF file");
                    bool exists = System.IO.Directory.Exists(attachmentLocation);

                    if (!exists)
                        System.IO.Directory.CreateDirectory(attachmentLocation);

                    File.Move(pdfFile, attachmentLocation + "\\SI" + invoiceName + ".pdf");
                } else
                {
                    Console.WriteLine("PDF file not exist");
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

