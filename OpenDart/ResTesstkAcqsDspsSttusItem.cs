using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResTesstkAcqsDspsSttusItem
    {
        [XmlElement("rcept_no")]
        public string rcept_no { get; set; }        // 접수번호	Y	접수번호(14자리)
                                                    // ※ 공시뷰어 연결에 이용예시
                                                    // - PC용 : http://dart.fss.or.kr/dsaf001/main.do?rcpNo=접수번호
                                                    // - 모바일용 : http://m.dart.fss.or.kr/html_mdart/MD1007.html?rcpNo=접수번호
        [XmlElement("corp_cls")]
        public string corp_cls { get; set; }	    // 법인구분	Y	법인구분 : Y(유가), K(코스닥), N(코넥스), E(기타)
        [XmlElement("corp_code")]
        public string corp_code { get; set; }        //	고유번호	Y	공시대상회사의 고유번호(8자리)
        [XmlElement("corp_name")]
        public string corp_name { get; set; }        //	법인명	Y	법인명
        [XmlElement("stock_knd")]
        public string stock_knd { get; set; }       //	주식 종류	Y	보통주, 우선주 등
        [XmlElement("acqs_mth1")]
        public string acqs_mth1 { get; set; }        //	취득방법 대분류	Y	배당가능이익범위 이내 취득, 기타취득, 총계 등
        [XmlElement("acqs_mth2")]
        public string acqs_mth2 { get; set; }        //	취득방법 중분류	Y	직접취득, 신탁계약에 의한취득, 기타취득, 총계 등
        [XmlElement("acqs_mth3")]
        public string acqs_mth3 { get; set; }       // 취득방법 소분류	Y	장내직접취득, 장외직접취득, 공개매수, 주식매수청구권행사, 수탁자보유물량,
                                                    // 현물보유량, 기타취득, 소계, 총계 등
        [XmlElement("bsis_qy")]
        public string bsis_qy { get; set; }         //	기초 수량	Y	9,999,999,999
        [XmlElement("change_qy_acqs")]
        public string change_qy_acqs { get; set; }  //	변동 수량 취득	Y	9,999,999,999
        [XmlElement("change_qy_dsps")]
        public string change_qy_dsps { get; set; }  //	변동 수량 처분	Y	9,999,999,999
        [XmlElement("change_qy_incnr")]
        public string change_qy_incnr { get; set; } //	변동 수량 소각	Y	9,999,999,999
        [XmlElement("trmend_qy")]
        public string trmend_qy { get; set; }       //	기말 수량	Y	9,999,999,999
        [XmlElement("rm")]
        public string rm { get; set; }              //	비고	Y	비고

        public ResTesstkAcqsDspsSttusItem()
        {
            rcept_no = "";
            corp_cls = "";
            corp_code = "";
            corp_name = "";
            stock_knd = "";
            acqs_mth1 = "";
            acqs_mth2 = "";
            acqs_mth3 = "";
            bsis_qy = "";
            change_qy_acqs = "";
            change_qy_dsps = "";
            change_qy_incnr = "";
            trmend_qy = "";
            rm = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResTesstkAcqsDspsSttusItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("stock_knd: {0}", stock_knd);
            Console.WriteLine("acqs_mth1: {0}", acqs_mth1);
            Console.WriteLine("acqs_mth2: {0}", acqs_mth2);
            Console.WriteLine("acqs_mth3: {0}", acqs_mth3);
            Console.WriteLine("bsis_qy: {0}", bsis_qy);
            Console.WriteLine("change_qy_acqs: {0}", change_qy_acqs);
            Console.WriteLine("change_qy_dsps: {0}", change_qy_dsps);
            Console.WriteLine("change_qy_incnr: {0}", change_qy_incnr);
            Console.WriteLine("trmend_qy: {0}", trmend_qy);
            Console.WriteLine("rm: {0}", rm);
            Console.WriteLine("==================================================");
        }
    }
}