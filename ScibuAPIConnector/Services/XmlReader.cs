namespace ScibuAPIConnector.Services
{
    using ScibuAPIConnector.Extensions;
    using ScibuAPIConnector.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;

    public class XmlReader
    {
        public ImportTable MapXml(string xmlName, string xmlFile) => 
            new ImportTable(xmlName, this.ReadColumns(xmlFile), this.ReadLines(xmlFile));

        public string[] ReadColumns(string fileName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileName);
            List<string> list2 = new List<string>();
            var enumerator = document.DocumentElement.SelectNodes("*").GetEnumerator();
                if (enumerator.MoveNext())
                {
                    foreach (XmlNode node2 in ((XmlNode) enumerator.Current).ChildNodes)
                    {
                        if (node2.Name != "bedrijf")
                        {
                            if (node2.Name == "contactpersoon")
                            {
                                continue;
                            }
                            list2.Add(node2.Name.RemoveSpecialCharacters());
                            continue;
                        }
                        foreach (XmlNode node3 in node2.ChildNodes)
                        {
                            list2.Add(node3.Name.RemoveSpecialCharacters());
                        }
                    }
                }
            return list2.ToArray();
        }

        public List<string[]> ReadLines(string fileName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileName);
            List<string[]> list2 = new List<string[]>();
            var enumerator = document.DocumentElement.SelectNodes("*").GetEnumerator();
                List<string> list3;
                goto TR_001C;
            TR_0009:
                list2.Add(list3.ToArray());
            TR_001C:
                while (true)
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    XmlNode current = (XmlNode) enumerator.Current;
                    list3 = new List<string>();
                    foreach (XmlNode node2 in current.ChildNodes)
                    {
                        if (node2.Name != "bedrijf")
                        {
                            if (node2.Name == "contactpersoon")
                            {
                                continue;
                            }
                            list3.Add(node2.InnerText.HtmlDecode().RemoveSpecialCharacters());
                            continue;
                        }
                        foreach (XmlNode node3 in node2.ChildNodes)
                        {
                            list3.Add(node3.InnerText.HtmlDecode().RemoveSpecialCharacters());
                        }
                    }
                    goto TR_0009;
                }
            
            return list2;
        }
    }
}

