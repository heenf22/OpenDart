using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    [XmlRoot("result")]
    public class ResEmpSttusResult
    {
        [XmlElement("status")]
        public string status { get; set; }          // 에러 및 정보 코드 (※메시지 설명 참조)
        [XmlElement("message")]
        public string message { get; set; }         // 에러 및 정보 메시지 (※메시지 설명 참조)
        [XmlElement("list")]
        public List<ResEmpSttusItem> list { get; set; }     // 증자(감자) 현황 목록

        public ResEmpSttusResult()
        {
            status = "";
            message = "";
            list = new List<ResEmpSttusItem>();
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResEmpSttusResult Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("corp_code: {0}", status);
            Console.WriteLine("bgn_de: {0}", message);
            foreach (ResEmpSttusItem item in list)
            {
                item.displayConsole();
            }
            Console.WriteLine("==================================================");
        }
    }
}