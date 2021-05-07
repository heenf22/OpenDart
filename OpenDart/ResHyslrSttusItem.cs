using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResHyslrSttusItem
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
        [XmlElement("stock_knd")]
        public string stock_knd { get; set; }       //	주식 종류	Y	보통주, 우선주 등
        [XmlElement("nm")]
        public string nm { get; set; }                  //	성명	Y	홍길동
        [XmlElement("relate")]
        public string relate { get; set; }                     //	관계	Y	본인, 친인척 등
        [XmlElement("bsis_posesn_stock_co")]
        public string bsis_posesn_stock_co { get; set; }       //	기초 소유 주식 수	Y	9,999,999,999
        [XmlElement("bsis_posesn_stock_qota_rt")]
        public string bsis_posesn_stock_qota_rt { get; set; }   //	기초 소유 주식 지분 율	Y	0.00
        [XmlElement("trmend_posesn_stock_co")]
        public string trmend_posesn_stock_co { get; set; }      //	기말 소유 주식 수	Y	9,999,999,999
        [XmlElement("trmend_posesn_stock_qota_rt")]
        public string trmend_posesn_stock_qota_rt { get; set; } //	기말 소유 주식 지분 율	Y	0.00
        [XmlElement("rm")]
        public string rm { get; set; }       //	비고	Y	비고

        public ResHyslrSttusItem()
        {
            rcept_no = "";
            corp_cls = "";
            corp_code = "";
            corp_name = "";
            stock_knd = "";
            nm = "";
            relate = "";
            bsis_posesn_stock_co = "";
            bsis_posesn_stock_qota_rt = "";
            trmend_posesn_stock_co = "";
            trmend_posesn_stock_qota_rt = "";
            rm = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResHyslrSttusItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("stock_knd: {0}", stock_knd);
            Console.WriteLine("nm: {0}", nm);
            Console.WriteLine("relate: {0}", relate);
            Console.WriteLine("bsis_posesn_stock_co: {0}", bsis_posesn_stock_co);
            Console.WriteLine("bsis_posesn_stock_qota_rt: {0}", bsis_posesn_stock_qota_rt);
            Console.WriteLine("trmend_posesn_stock_co: {0}", trmend_posesn_stock_co);
            Console.WriteLine("trmend_posesn_stock_qota_rt: {0}", trmend_posesn_stock_qota_rt);
            Console.WriteLine("rm: {0}", rm);
            Console.WriteLine("==================================================");
        }
    }
}