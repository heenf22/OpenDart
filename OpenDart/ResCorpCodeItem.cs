using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResCorpCodeItem
    {
        [XmlElement("corp_code")]
        public string corp_code { get; set; }
        [XmlElement("corp_name")]
        public string corp_name { get; set; }
        [XmlElement("stock_code")]
        public string stock_code { get; set; }
        [XmlElement("modify_date")]
        public string modify_date { get; set; }

        public ResCorpCodeItem()
        {
            corp_code = "";
            corp_name = "";
            stock_code = "";
            modify_date = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResCorpCodeItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("stock_code: {0}", stock_code);
            Console.WriteLine("modify_date: {0}", modify_date);
            Console.WriteLine("==================================================");
        }
    }
}