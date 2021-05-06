using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResIrdsSttusItem
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
        [XmlElement("isu_dcrs_de")]
        public string isu_dcrs_de { get; set; }    //	주식발행 감소일자	Y	주식발행 감소일자
        [XmlElement("isu_dcrs_stle")]
        public string isu_dcrs_stle { get; set; }        //	발행 감소 형태	Y	발행 감소 형태
        [XmlElement("isu_dcrs_stock_knd")]
        public string isu_dcrs_stock_knd { get; set; }   //	발행 감소 주식 종류	Y	발행 감소 주식 종류
        [XmlElement("isu_dcrs_qy")]
        public string isu_dcrs_qy { get; set; }          //	발행 감소 수량	Y	9,999,999,999
        [XmlElement("isu_dcrs_mstvdv_fval_amount")]
        public string isu_dcrs_mstvdv_fval_amount { get; set; }     // 발행 감소 주당 액면 가액	Y	9,999,999,999
        [XmlElement("isu_dcrs_mstvdv_amount")]
        public string isu_dcrs_mstvdv_amount { get; set; }          // 발행 감소 주당 가액	Y	9,999,999,999

        public ResIrdsSttusItem()
        {
            rcept_no = "";
            corp_cls = "";
            corp_code = "";
            corp_name = "";
            isu_dcrs_de = "";
            isu_dcrs_stle = "";
            isu_dcrs_stock_knd = "";
            isu_dcrs_qy = "";
            isu_dcrs_mstvdv_fval_amount = "";
            isu_dcrs_mstvdv_amount = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResIrdsSttusItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("isu_dcrs_de: {0}", isu_dcrs_de);
            Console.WriteLine("isu_dcrs_stle: {0}", isu_dcrs_stle);
            Console.WriteLine("isu_dcrs_stock_knd: {0}", isu_dcrs_stock_knd);
            Console.WriteLine("isu_dcrs_qy: {0}", isu_dcrs_qy);
            Console.WriteLine("isu_dcrs_mstvdv_fval_amount: {0}", isu_dcrs_mstvdv_fval_amount);
            Console.WriteLine("isu_dcrs_mstvdv_amount: {0}", isu_dcrs_mstvdv_amount);
            Console.WriteLine("==================================================");
        }
    }
}