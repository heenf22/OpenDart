using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResExctvSttusItem
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
        public string nm { get; set; }       //	성명	Y	홍길동
        [XmlElement("sexdstn")]
        public string sexdstn { get; set; }       //	성별	Y	남
        [XmlElement("birth_ym")]
        public string birth_ym { get; set; }       //	출생 년월	Y	YYYY년 MM월
        [XmlElement("ofcps")]
        public string ofcps { get; set; }       //	직위	Y	회장, 사장, 사외이사 등
        [XmlElement("rgist_exctv_at")]
        public string rgist_exctv_at { get; set; }       //	등기 임원 여부	Y	등기임원, 미등기임원 등
        [XmlElement("fte_at")]
        public string fte_at { get; set; }       //	상근 여부	Y	상근, 비상근
        [XmlElement("chrg_job")]
        public string chrg_job { get; set; }       //	담당 업무	Y	대표이사, 이사, 사외이사 등
        [XmlElement("main_career")]
        public string main_career { get; set; }       //	주요 경력	Y	
        [XmlElement("mxmm_shrholdr_relate")]
        public string mxmm_shrholdr_relate { get; set; }       //	최대 주주 관계	Y	
        [XmlElement("hffc_pd")]
        public string hffc_pd { get; set; }       //	재직 기간	Y	
        [XmlElement("tenure_end_on")]
        public string tenure_end_on { get; set; }       //	임기 만료 일	Y	

        public ResExctvSttusItem()
        {
            rcept_no = "";
            corp_cls = "";
            corp_code = "";
            corp_name = "";
            nm = "";
            sexdstn = "";
            birth_ym = "";
            ofcps = "";
            rgist_exctv_at = "";
            fte_at = "";
            chrg_job = "";
            main_career = "";
            mxmm_shrholdr_relate = "";
            hffc_pd = "";
            tenure_end_on = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResExctvSttusItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("nm: {0}", nm);
            Console.WriteLine("sexdstn: {0}", sexdstn);
            Console.WriteLine("birth_ym: {0}", birth_ym);
            Console.WriteLine("ofcps: {0}", ofcps);
            Console.WriteLine("rgist_exctv_at: {0}", rgist_exctv_at);
            Console.WriteLine("fte_at: {0}", fte_at);
            Console.WriteLine("chrg_job: {0}", chrg_job);
            Console.WriteLine("main_career: {0}", main_career);
            Console.WriteLine("mxmm_shrholdr_relate: {0}", mxmm_shrholdr_relate);
            Console.WriteLine("hffc_pd: {0}", hffc_pd);
            Console.WriteLine("tenure_end_on: {0}", tenure_end_on);
            Console.WriteLine("==================================================");
        }
    }
}