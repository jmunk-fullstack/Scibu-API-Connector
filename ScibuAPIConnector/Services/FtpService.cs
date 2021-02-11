using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ScibuAPIConnector.CustomFunctions;

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
                        var downloadAll = false;
                        if (UploadSettings.DatabaseName == "techneaportal" || UploadSettings.DatabaseName == "techneatestportal")
                        {
                            if (str.Contains("Offerteregels"))
                            {
                                str2 = "offertes/Offerteregels";
                            }
                            else if (str.Contains("Offertes"))
                            {
                                str2 = "offertes/Offertes";
                            }
                            else if (str.Contains("Facturen"))
                            {
                                downloadAll = true;
                            }
                        }
                        string uploadType = UploadSettings.UploadType;
                        if (uploadType == "CSV")
                        {
                            uploadType = "csv";
                        }
                        Console.WriteLine("Downloading files from the FTP server... ");
                        string path = UploadSettings.ImportLocation + str + "." + UploadSettings.UploadType;
                        string address = UploadSettings.FilesUrl + str2 + "." + uploadType;

                        if (downloadAll == true)
                        {
                            Copy(UploadSettings.FilesUrl, UploadSettings.ImportLocation);
                        }
                        else
                        {
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
                                    }
                                    else
                                    {
                                        SendHKVMail(str);
                                        return;
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Ftp files not found.");
                                    Console.WriteLine(ex);
                                    if(UploadSettings.SendHkvMail == "true")
                                    {
                                        SendHKVMail(str);
                                    }
                                    return;
                                }
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

        public void SendHKVMail(string str)
        {
            //Mail naar Maarten, ftp files niet gevonden.
            if (UploadSettings.DatabaseName == "hkvportal" && UploadSettings.SendHkvMail == "true")
            {
                var HKV = new HKV();
                var body = $"Er is een fout opgetreden tijdens de import van HKV naar Scibu. Het ftp bestand {str} is niet aanwezig.";
                HKV.SendMail(body, "Fout tijdens import scibu!");
            }
        }


        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}

