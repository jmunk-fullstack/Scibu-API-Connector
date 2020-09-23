namespace ScibuAPIConnector.Services
{
    using ScibuAPIConnector.Extensions;
    using ScibuAPIConnector.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    public class TechneaXMLReader
    {
        public List<string> allInvoiceHeaders = new List<string>();
        public List<string> allInvoiceLineHeaders = new List<string>();
        public List<string> allInvoiceResult = new List<string>();
        public string[] allInvoiceLineResult;

        public List<ImportTable> GetImportTables(string xmlFile)
        {
            allInvoiceHeaders.Clear();
            allInvoiceLineHeaders.Clear();
            allInvoiceResult.Clear();
            allInvoiceLineResult = new string[] { };

            var importTables = new List<ImportTable>();

            var invoiceImportTable = new ImportTable("Facturen", ReadInvoiceColumns(xmlFile).ToArray(), ReadInvoiceResult(xmlFile));
            var invoiceNumber = invoiceImportTable.Rows[0][0];
            var invoiceProductImportTable = new ImportTable("Factuurregels", ReadInvoiceLineColumns(xmlFile).ToArray(), ReadInvoiceLineResult(xmlFile, invoiceNumber));

            importTables.Add(invoiceImportTable);
            importTables.Add(invoiceProductImportTable);

            return importTables;
        }

        public void ReadInvoiceLineResultRecursive(XmlNodeList nodes, string name)
        {
            foreach (XmlNode header in nodes)
            {
                if (header.Name != "#text")
                {
                    var newName = header.Name.HtmlDecode().RemoveSpecialCharacters().ToString();
                    if (name != "")
                    {
                        newName = name + "/" + header.Name.HtmlDecode().RemoveSpecialCharacters().ToString();
                    }

                    var headerIndex = allInvoiceLineHeaders.FindIndex(x => x == newName);
                    allInvoiceLineResult[headerIndex] = header.InnerText.HtmlDecode().RemoveSpecialCharacters().ToString();
                    ReadInvoiceLineResultRecursive(header.ChildNodes, newName);
                }
            }
        }

        public void ReadInvoiceResultRecursive(XmlNodeList nodes)
        {
            foreach (XmlNode header in nodes)
            {
                if (header.Name != "#text" && header.Name != "InvLines")
                {
                    allInvoiceResult.Add(header.InnerText.HtmlDecode().RemoveSpecialCharacters().ToString());
                    ReadInvoiceResultRecursive(header.ChildNodes);
                }
            }
        }

        public void ReadInvoiceRecursive(XmlNodeList nodes, string name)
        {
            foreach (XmlNode header in nodes)
            {
                if (header.Name != "#text" && header.Name != "InvLines")
                {
                    var newName = header.Name.HtmlDecode().RemoveSpecialCharacters().ToString();
                    if (name != "")
                    {
                        newName = name + "/" + header.Name.HtmlDecode().RemoveSpecialCharacters().ToString();
                    }
                    allInvoiceHeaders.Add(newName);
                    ReadInvoiceRecursive(header.ChildNodes, newName);
                }
            }
        }

        public void ReadInvoiceLineRecursive(XmlNodeList nodes, string name)
        {
            foreach (XmlNode header in nodes)
            {
                if (header.Name != "#text")
                {
                    var newName = header.Name.HtmlDecode().RemoveSpecialCharacters().ToString();
                    if (name != "")
                    {
                        newName = name + "/" + header.Name.HtmlDecode().RemoveSpecialCharacters().ToString();
                    }

                    if (!allInvoiceLineHeaders.Contains(newName))
                        allInvoiceLineHeaders.Add(newName);

                    ReadInvoiceLineRecursive(header.ChildNodes, newName);
                }
            }
        }

        public List<string[]> ReadInvoiceResult(string fileName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            var nodes = document.GetElementsByTagName("InvHeader");

            ReadInvoiceResultRecursive(nodes[0].ChildNodes);

            var listInvoiceLines = new List<string[]>();
            listInvoiceLines.Add(allInvoiceResult.ToArray());

            return listInvoiceLines;
        }

        public List<string> ReadInvoiceColumns(string fileName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            var nodes = document.GetElementsByTagName("InvHeader");

            ReadInvoiceRecursive(nodes[0].ChildNodes, "");

            return allInvoiceHeaders;
        }

        public List<string[]> ReadInvoiceLineResult(string fileName, string invoiceNumber)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            var nodes = document.GetElementsByTagName("InvLines");

            var listInvoiceLines = new List<string[]>();

            foreach (XmlNode node in nodes)
            {
                allInvoiceLineResult = new string[allInvoiceLineHeaders.Count];
                ReadInvoiceLineResultRecursive(node.ChildNodes, "");
                allInvoiceLineResult[allInvoiceLineHeaders.Count - 1] = invoiceNumber;
                listInvoiceLines.Add(allInvoiceLineResult);
            }

            return listInvoiceLines;
        }

        public List<string> ReadInvoiceLineColumns(string fileName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            var nodes = document.GetElementsByTagName("InvLines");
            foreach (XmlNode node in nodes)
            {
                ReadInvoiceLineRecursive(node.ChildNodes, "");
            }
            allInvoiceLineHeaders.Add("Factuurnummer");
            return allInvoiceLineHeaders;
        }
    }
}

