using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResHmvAuditIndvdlBySttusItem
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
        [XmlElement("nm")]
        public string nm { get; set; }       //	이름	Y	홍길동
        [XmlElement("ofcps")]
        public string ofcps { get; set; }       //	직위	Y	이사, 대표이사 등
        [XmlElement("mendng_totamt")]
        public string mendng_totamt { get; set; }       //	보수 총액	Y	9,999,999,999
        [XmlElement("mendng_totamt_ct_incls_mendng")]
        public string mendng_totamt_ct_incls_mendng { get; set; }       //	보수 총액 비 포함 보수	Y	9,999,999,999

        public ResHmvAuditIndvdlBySttusItem()
        {
            rcept_no = "";
            corp_cls = "";
            corp_code = "";
            corp_name = "";
            nm = "";
            ofcps = "";
            mendng_totamt = "";
            mendng_totamt_ct_incls_mendng = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResHmvAuditIndvdlBySttusItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("nm: {0}", nm);
            Console.WriteLine("ofcps: {0}", ofcps);
            Console.WriteLine("mendng_totamt: {0}", mendng_totamt);
            Console.WriteLine("mendng_totamt_ct_incls_mendng: {0}", mendng_totamt_ct_incls_mendng);
            Console.WriteLine("==================================================");
        }
    }
}