using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResHmvAuditAllSttusItem
    {
        [XmlElement("rcept_no")]
        public string rcept_no { get; set; }        // 접수번호	Y	접수번호(14자리)
                                                    // ※ 공시뷰어 연결에 이용예시
                                                    // - PC용 : http://dart.fss.or.kr/dsaf001/main.do?rcpNo=접수번호
                                                    // - 모바일용 : http://m.dart.fss.or.kr/html_mdart/MD1007.html?rcpNo=접수번호
        [XmlElement("corp_cls")]
        public string corp_cls { get; set; }	    // 법인구분	Y	법인구분 : Y(유가), K(코스닥), N(코넥스), E(기타)
        [XmlElement("corp_code")]
        public string corp_code { get; set; }       //	고유번호	Y	공시대상회사의 고유번호(8자리)
        [XmlElement("corp_name")]
        public string corp_name { get; set; }       //	법인명	Y	법인명
        [XmlElement("nmpr")]
        public string nmpr { get; set; }       //	인원수	Y	9,999,999,999
        [XmlElement("mendng_totamt")]
        public string mendng_totamt { get; set; }       //	보수 총액	Y	9,999,999,999
        [XmlElement("jan_avrg_mendng_am")]
        public string jan_avrg_mendng_am { get; set; }       //	1인 평균 보수 액	Y	9,999,999,999
        [XmlElement("rm")]
        public string rm { get; set; }       //	비고	Y	

        public ResHmvAuditAllSttusItem()
        {
            rcept_no = "";
            corp_cls = "";
            corp_code = "";
            corp_name = "";
            nmpr = "";
            mendng_totamt = "";
            jan_avrg_mendng_am = "";
            rm = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResHmvAuditAllSttusItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("nmpr: {0}", nmpr);
            Console.WriteLine("mendng_totamt: {0}", mendng_totamt);
            Console.WriteLine("jan_avrg_mendng_am: {0}", jan_avrg_mendng_am);
            Console.WriteLine("rm: {0}", rm);
            Console.WriteLine("==================================================");
        }
    }
}