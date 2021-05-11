using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResElestockItem
    {
        [XmlElement("rcept_no")]
        public string rcept_no { get; set; }        // 접수번호	Y	접수번호(14자리)
                                                    // ※ 공시뷰어 연결에 이용예시
                                                    // - PC용 : http://dart.fss.or.kr/dsaf001/main.do?rcpNo=접수번호
                                                    // - 모바일용 : http://m.dart.fss.or.kr/html_mdart/MD1007.html?rcpNo=접수번호
        [XmlElement("rcept_dt")]
        public string rcept_dt { get; set; }        //	접수일자	Y	공시 접수일자(YYYYMMDD)
        [XmlElement("corp_code")]
        public string corp_code { get; set; }       //	고유번호	Y	공시대상회사의 고유번호(8자리)
        [XmlElement("corp_name")]
        public string corp_name { get; set; }       //	회사명	Y	회사명
        [XmlElement("reportr")]
        public string reportr { get; set; }         //	보고자	Y	보고자명
        [XmlElement("isu_exctv_rgist_at")]
        public string isu_exctv_rgist_at { get; set; }      //	발행 회사 관계 임원(등기여부)	Y	등기임원, 비등기임원 등
        [XmlElement("isu_exctv_ofcps")]
        public string isu_exctv_ofcps { get; set; }         //	발행 회사 관계 임원 직위	Y	대표이사, 이사, 전무 등
        [XmlElement("isu_main_shrholdr")]
        public string isu_main_shrholdr { get; set; }       //	발행 회사 관계 주요 주주	Y	10%이상주주 등
        [XmlElement("sp_stock_lmp_cnt")]
        public string sp_stock_lmp_cnt { get; set; }        //	특정 증권 등 소유 수	Y	9,999,999,999
        [XmlElement("sp_stock_lmp_irds_cnt")]
        public string sp_stock_lmp_irds_cnt { get; set; }   //	특정 증권 등 소유 증감 수	Y	9,999,999,999
        [XmlElement("sp_stock_lmp_rate")]
        public string sp_stock_lmp_rate { get; set; }       //	특정 증권 등 소유 비율	Y	0.00
        [XmlElement("sp_stock_lmp_irds_rate")]
        public string sp_stock_lmp_irds_rate { get; set; }  //	특정 증권 등 소유 증감 비율	Y	0.00

        public ResElestockItem()
        {
            rcept_no = "";
            rcept_dt = "";
            corp_code = "";
            corp_name = "";
            reportr = "";
            isu_exctv_rgist_at = "";
            isu_exctv_ofcps = "";
            isu_main_shrholdr = "";
            sp_stock_lmp_cnt = "";
            sp_stock_lmp_irds_cnt = "";
            sp_stock_lmp_rate = "";
            sp_stock_lmp_irds_rate = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResElestockItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("rcept_dt: {0}", rcept_dt);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("reportr: {0}", reportr);
            Console.WriteLine("isu_exctv_rgist_at: {0}", isu_exctv_rgist_at);
            Console.WriteLine("isu_exctv_ofcps: {0}", isu_exctv_ofcps);
            Console.WriteLine("isu_main_shrholdr: {0}", isu_main_shrholdr);
            Console.WriteLine("sp_stock_lmp_cnt: {0}", sp_stock_lmp_cnt);
            Console.WriteLine("sp_stock_lmp_irds_cnt: {0}", sp_stock_lmp_irds_cnt);
            Console.WriteLine("sp_stock_lmp_rate: {0}", sp_stock_lmp_rate);
            Console.WriteLine("sp_stock_lmp_irds_rate: {0}", sp_stock_lmp_irds_rate);
            Console.WriteLine("==================================================");
        }
    }
}