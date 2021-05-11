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

    [XmlRoot("result")]
    public class ResCorpCodeResult
    {
        [XmlElement("list")]
        public List<ResCorpCodeItem> Dummys { get; set; }
    }
}