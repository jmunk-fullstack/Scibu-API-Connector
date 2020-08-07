using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ScibuAPIConnector.Services
{
    public class FtpService
    {
        public void DownloadFiles()
        {
            string[] uploadFiles;
            int num;
            if (UploadSettings.DownloadFiles != "true")
            {
                return;
            }
            else
            {
                uploadFiles = UploadSettings.UploadFiles;
                num = 0;
            }
            while (true)
            {
                while (true)
                {
                    if (num < uploadFiles.Length)
                    {
                        string str = uploadFiles[num];
                        string str2 = str;
                        if (UploadSettings.DatabaseName == "techneaportal")
                        {
                            if (str.Contains("Offerteregels"))
                            {
                                str2 = "offertes/Offerteregels";
                            }
                            else if (str.Contains("Offertes"))
                            {
                                str2 = "offertes/Offertes";
                            }
                        }
                        string uploadType = UploadSettings.UploadType;
                        if (uploadType == "CSV")
                        {
                            uploadType = "csv";
                        }
                        Console.WriteLine("Downloading files from the FTP server...");
                        string path = UploadSettings.ImportLocation + str + "." + UploadSettings.UploadType;
                        string address = UploadSettings.FilesUrl + str2 + "." + uploadType;
                        using (WebClient client = new WebClient())
                        {
                            try
                            {
                                if (File.Exists(address))
                                {
                                    byte[] buffer = client.DownloadData(address);
                                    if (File.Exists(path))
                                    {
                                        File.Delete(path);
                                    }
                                    using (FileStream stream = File.Create(path))
                                    {
                                        stream.Write(buffer, 0, buffer.Length);
                                        stream.Close();
                                    }
                                } else
                                {
                                    return;
                                }

                            } catch(Exception ex)
                            {
                                Console.WriteLine("Ftp files not found.");
                                return;
                            }
                        }

                        if (UploadSettings.DeleteFTPFiles == "true")
                        {
                            File.Delete(address);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Downloading done!");
                        return;
                    }
                    break;
                }
                num++;
            }
        }
    }
}
