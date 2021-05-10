using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResEmpSttusItem
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
        [XmlElement("fo_bbm")]
        public string fo_bbm { get; set; }       //	사 업부문	Y	
        [XmlElement("sexdstn")]
        public string sexdstn { get; set; }       //	성별	Y	남, 여
        [XmlElement("reform_bfe_emp_co_rgllbr")]
        public string reform_bfe_emp_co_rgllbr { get; set; }       //	개정 전 직원 수 정규직	Y	
        [XmlElement("reform_bfe_emp_co_cnttk")]
        public string reform_bfe_emp_co_cnttk { get; set; }       //	개정 전 직원 수 계약직	Y	
        [XmlElement("reform_bfe_emp_co_etc")]
        public string reform_bfe_emp_co_etc { get; set; }       //	개정 전 직원 수 기타	Y	
        [XmlElement("rgllbr_co")]
        public string rgllbr_co { get; set; }       //	정규직 수	Y	상근, 비상근
        [XmlElement("rgllbr_abacpt_labrr_co")]
        public string rgllbr_abacpt_labrr_co { get; set; }       //	정규직 단시간 근로자 수	Y	대표이사, 이사, 사외이사 등
        [XmlElement("cnttk_co")]
        public string cnttk_co { get; set; }       //	계약직 수	Y	9,999,999,999
        [XmlElement("cnttk_abacpt_labrr_co")]
        public string cnttk_abacpt_labrr_co { get; set; }       //	계약직 단시간 근로자 수	Y	9,999,999,999
        [XmlElement("sm")]
        public string sm { get; set; }       //	합계	Y	9,999,999,999
        [XmlElement("avrg_cnwk_sdytrn")]
        public string avrg_cnwk_sdytrn { get; set; }       //	평균 근속 연수	Y	9,999,999,999
        [XmlElement("fyer_salary_totamt")]
        public string fyer_salary_totamt { get; set; }       //	연간 급여 총액	Y	9,999,999,999
        [XmlElement("jan_salary_am")]
        public string jan_salary_am { get; set; }       //	1인평균 급여 액	Y	9,999,999,999
        [XmlElement("rm")]
        public string rm { get; set; }       //	비고	Y	

        public ResEmpSttusItem()
        {
            rcept_no = "";
            corp_cls = "";
            corp_code = "";
            corp_name = "";
            fo_bbm = "";
            sexdstn = "";
            reform_bfe_emp_co_rgllbr = "";
            reform_bfe_emp_co_cnttk = "";
            reform_bfe_emp_co_etc = "";
            rgllbr_co = "";
            rgllbr_abacpt_labrr_co = "";
            cnttk_co = "";
            cnttk_abacpt_labrr_co = "";
            sm = "";
            avrg_cnwk_sdytrn = "";
            fyer_salary_totamt = "";
            jan_salary_am = "";
            rm = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResEmpSttusItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("fo_bbm: {0}", fo_bbm);
            Console.WriteLine("sexdstn: {0}", sexdstn);
            Console.WriteLine("reform_bfe_emp_co_rgllbr: {0}", reform_bfe_emp_co_rgllbr);
            Console.WriteLine("reform_bfe_emp_co_cnttk: {0}", reform_bfe_emp_co_cnttk);
            Console.WriteLine("reform_bfe_emp_co_etc: {0}", reform_bfe_emp_co_etc);
            Console.WriteLine("rgllbr_co: {0}", rgllbr_co);
            Console.WriteLine("rgllbr_abacpt_labrr_co: {0}", rgllbr_abacpt_labrr_co);
            Console.WriteLine("cnttk_co: {0}", cnttk_co);
            Console.WriteLine("cnttk_abacpt_labrr_co: {0}", cnttk_abacpt_labrr_co);
            Console.WriteLine("sm: {0}", sm);
            Console.WriteLine("avrg_cnwk_sdytrn: {0}", avrg_cnwk_sdytrn);
            Console.WriteLine("fyer_salary_totamt: {0}", fyer_salary_totamt);
            Console.WriteLine("jan_salary_am: {0}", jan_salary_am);
            Console.WriteLine("rm: {0}", rm);
            Console.WriteLine("==================================================");
        }
    }
}