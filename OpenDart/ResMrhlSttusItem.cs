using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResMrhlSttusItem
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
        [XmlElement("se")]
        public string se { get; set; }                  //	구분	Y	소액주주
        [XmlElement("shrholdr_co")]
        public string shrholdr_co { get; set; }         //	주주수	Y	9,999,999,999
        [XmlElement("shrholdr_tot_co")]
        public string shrholdr_tot_co { get; set; }     //	전체 주주수	Y	9,999,999,999
        [XmlElement("shrholdr_rate")]
        public string shrholdr_rate { get; set; }       //	주주 비율	Y	0.00
        [XmlElement("hold_stock_co")]
        public string hold_stock_co { get; set; }       //	보유 주식수	Y	9,999,999,999
        [XmlElement("stock_tot_co")]
        public string stock_tot_co { get; set; }        //	총발행 주식수	Y	9,999,999,999
        [XmlElement("hold_stock_rate")]
        public string hold_stock_rate { get; set; }     //	보유 주식 비율	Y	0.00

        public ResMrhlSttusItem()
        {
            rcept_no = "";
            corp_cls = "";
            corp_code = "";
            corp_name = "";
            se = "";
            shrholdr_co = "";
            shrholdr_tot_co = "";
            shrholdr_rate = "";
            hold_stock_co = "";
            stock_tot_co = "";
            hold_stock_rate = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResMrhlSttusItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("se: {0}", se);
            Console.WriteLine("shrholdr_co: {0}", shrholdr_co);
            Console.WriteLine("shrholdr_tot_co: {0}", shrholdr_tot_co);
            Console.WriteLine("shrholdr_rate: {0}", shrholdr_rate);
            Console.WriteLine("hold_stock_co: {0}", hold_stock_co);
            Console.WriteLine("hold_stock_rate: {0}", hold_stock_rate);
            Console.WriteLine("==================================================");
        }
    }
}