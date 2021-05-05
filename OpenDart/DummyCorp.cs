using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    // CORPCODE.xml
    //<? xml version="1.0" encoding="UTF-8"?>
    //<result>
    //    <list>
    //        <corp_code>00434003</corp_code>
    //        <corp_name>다코</corp_name>
    //        <stock_code> </stock_code>
    //        <modify_date>20170630</modify_date>
    //    </list>
    //    ...
    //    <list>
    //        <corp_code>00434456</corp_code>
    //        <corp_name>일산약품</corp_name>
    //        <stock_code> </stock_code>
    //        <modify_date>20170630</modify_date>
    //    </list>
    //</result>

    public class DummyCorp
    {
        [XmlElement("corp_code")]
        public string corp_code { get; set; }
        [XmlElement("corp_name")]
        public string corp_name { get; set; }
        [XmlElement("stock_code")]
        public string stock_code { get; set; }
        [XmlElement("modify_date")]
        public string modify_date { get; set; }

        public DummyCorp()
        {
            corp_code = "";
            corp_name = "";
            stock_code = "";
            modify_date = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("Corp Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("stock_code: {0}", stock_code);
            Console.WriteLine("modify_date: {0}", modify_date);
            Console.WriteLine("==================================================");
        }
    }
}