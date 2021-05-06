using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResAlotMatterItem
    {
        [XmlElement("rcept_no")]
        public string rcept_no { get; set; }        // 접수번호	Y	접수번호(14자리)
                                                    // ※ 공시뷰어 연결에 이용예시
                                                    // - PC용 : http://dart.fss.or.kr/dsaf001/main.do?rcpNo=접수번호
                                                    // - 모바일용 : http://m.dart.fss.or.kr/html_mdart/MD1007.html?rcpNo=접수번호
        [XmlElement("rcept_no")]
        public string corp_cls { get; set; }	    // 법인구분	Y	법인구분 : Y(유가), K(코스닥), N(코넥스), E(기타)
        [XmlElement("corp_code")]
        public string corp_code { get; set; }       //	고유번호	Y	공시대상회사의 고유번호(8자리)
        [XmlElement("corp_name")]
        public string corp_name { get; set; }       //	법인명	Y	법인명
        [XmlElement("se")]
        public string se { get; set; }              //	구분	Y	유상증자(주주배정), 전환권행사 등
        [XmlElement("stock_knd")]
        public string stock_knd { get; set; }       //	주식 종류	Y	보통주 등
        [XmlElement("thstrm")]
        public string thstrm { get; set; }          //	당기	Y	9,999,999,999
        [XmlElement("frmtrm")]
        public string frmtrm { get; set; }          //	전기	Y	9,999,999,999
        [XmlElement("lwfr")]
        public string lwfr { get; set; }            //	전전기	Y	9,999,999,999

        public ResAlotMatterItem()
        {
            rcept_no = "";
            corp_cls = "";
            corp_code = "";
            corp_name = "";
            se = "";
            stock_knd = "";
            thstrm = "";
            frmtrm = "";
            lwfr = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResAlotMatterItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("se: {0}", se);
            Console.WriteLine("stock_knd: {0}", stock_knd);
            Console.WriteLine("thstrm: {0}", thstrm);
            Console.WriteLine("frmtrm: {0}", frmtrm);
            Console.WriteLine("lwfr: {0}", lwfr);
            Console.WriteLine("==================================================");
        }
    }
}