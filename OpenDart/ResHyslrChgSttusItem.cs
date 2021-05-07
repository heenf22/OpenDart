using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResHyslrChgSttusItem
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
        [XmlElement("change_on")]
        public string change_on { get; set; }       //	변동 일	Y	YYYY.MM.DD
        [XmlElement("mxmm_shrholdr_nm")]
        public string mxmm_shrholdr_nm { get; set; }       //	최대 주주 명	Y	홍길동
        [XmlElement("posesn_stock_co")]
        public string posesn_stock_co { get; set; }       //	소유 주식 수	Y	9,999,999,999
        [XmlElement("qota_rt")]
        public string qota_rt { get; set; }             //	지분 율	Y	0.00
        [XmlElement("change_cause")]
        public string change_cause { get; set; }        //	변동 원인	Y	
        [XmlElement("rm")]
        public string rm { get; set; }                  //	비고	Y	

        public ResHyslrChgSttusItem()
        {
            rcept_no = "";
            corp_cls = "";
            corp_code = "";
            corp_name = "";
            change_on = "";
            mxmm_shrholdr_nm = "";
            posesn_stock_co = "";
            qota_rt = "";
            change_cause = "";
            rm = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResHyslrChgSttusItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("change_on: {0}", change_on);
            Console.WriteLine("mxmm_shrholdr_nm: {0}", mxmm_shrholdr_nm);
            Console.WriteLine("posesn_stock_co: {0}", posesn_stock_co);
            Console.WriteLine("qota_rt: {0}", qota_rt);
            Console.WriteLine("change_cause: {0}", change_cause);
            Console.WriteLine("rm: {0}", rm);
            Console.WriteLine("==================================================");
        }
    }
}