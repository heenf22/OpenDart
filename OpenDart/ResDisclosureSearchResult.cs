using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    [XmlRoot("result")]
    public class ResDisclosureSearchResult
    {
        [XmlElement("status")]
        public string status { get; set; }          // 에러 및 정보 코드 (※메시지 설명 참조)
        [XmlElement("message")]
        public string message { get; set; }         // 에러 및 정보 메시지 (※메시지 설명 참조)
        [XmlElement("page_no")]
        public int page_no { get; set; }            // 페이지 번호
        [XmlElement("page_count")]
        public int page_count { get; set; }         // 페이지 별 건수
        [XmlElement("total_count")]
        public int total_count { get; set; }        // 총 건수
        [XmlElement("total_page")]
        public int total_page { get; set; }         // 총 페이지 수
        [XmlElement("list")]
        public List<ResDisclosureSearchItem> list { get; set; }     // 공시 목록

        public ResDisclosureSearchResult()
        {
            status = "";
            message = "";
            page_no = 0;
            page_count = 0;
            total_count = 0;
            total_page = 0;
            list = new List<ResDisclosureSearchItem>();
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResDisclosureSearchResult Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("corp_code: {0}", status);
            Console.WriteLine("bgn_de: {0}", message);
            Console.WriteLine("end_de: {0}", page_no);
            Console.WriteLine("last_reprt_at: {0}", page_count);
            Console.WriteLine("pblntf_ty: {0}", total_count);
            Console.WriteLine("pblntf_detail_ty: {0}", total_page);
            foreach (ResDisclosureSearchItem item in list)
            {
                item.displayConsole();
            }
            Console.WriteLine("==================================================");
        }
    }
}