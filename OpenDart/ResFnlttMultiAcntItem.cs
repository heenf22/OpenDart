using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResFnlttMultiAcntItem
    {
        [XmlElement("rcept_no")]
        public string rcept_no { get; set; }        // 접수번호	Y	접수번호(14자리)
                                                    // ※ 공시뷰어 연결에 이용예시
                                                    // - PC용 : http://dart.fss.or.kr/dsaf001/main.do?rcpNo=접수번호
                                                    // - 모바일용 : http://m.dart.fss.or.kr/html_mdart/MD1007.html?rcpNo=접수번호
        [XmlElement("bsns_year")]
        public string bsns_year { get; set; }        //	사업 연도	Y	2019
        [XmlElement("stock_code")]
        public string stock_code { get; set; }        //	종목 코드	Y	상장회사의 종목코드(6자리)
        [XmlElement("reprt_code")]
        public string reprt_code { get; set; }        //	보고서 코드	Y	1분기보고서 : 11013
                                                        // 반기보고서 : 11012
                                                        // 3분기보고서 : 11014
                                                        // 사업보고서 : 11011
        [XmlElement("account_nm")]
        public string account_nm { get; set; }        //	계정명	Y	ex) 자본총계
        [XmlElement("fs_div")]
        public string fs_div { get; set; }        //	개별/연결구분	Y	CFS:연결재무제표, OFS:재무제표
        [XmlElement("fs_nm")]
        public string fs_nm { get; set; }        //	개별/연결명	Y	ex) 연결재무제표 또는 재무제표 출력
        [XmlElement("sj_div")]
        public string sj_div { get; set; }        //	재무제표구분	Y	BS:재무상태표, IS:손익계산서
        [XmlElement("sj_nm")]
        public string sj_nm { get; set; }        //	재무제표명	Y	ex) 재무상태표 또는 손익계산서 출력
        [XmlElement("thstrm_nm")]
        public string thstrm_nm { get; set; }        //	당기명	Y	ex) 제 13 기 3분기말
        [XmlElement("thstrm_dt")]
        public string thstrm_dt { get; set; }        //	당기일자	Y	ex) 2018.09.30 현재
        [XmlElement("thstrm_amount")]
        public string thstrm_amount { get; set; }        //	당기금액	Y	9,999,999,999
        [XmlElement("thstrm_add_amount")]
        public string thstrm_add_amount { get; set; }        //	당기누적금액	Y	9,999,999,999
        [XmlElement("frmtrm_nm")]
        public string frmtrm_nm { get; set; }        //	전기명	Y	ex) 제 12 기말
        [XmlElement("frmtrm_dt")]
        public string frmtrm_dt { get; set; }        //	전기일자	Y	ex) 2017.01.01 ~ 2017.12.31
        [XmlElement("frmtrm_amount")]
        public string frmtrm_amount { get; set; }        //	전기금액	Y	9,999,999,999
        [XmlElement("frmtrm_add_amount")]
        public string frmtrm_add_amount { get; set; }        //	전기누적금액	Y	9,999,999,999
        [XmlElement("bfefrmtrm_nm")]
        public string bfefrmtrm_nm { get; set; }        //	전전기명	Y	ex) 제 11 기말(※ 사업보고서의 경우에만 출력)
        [XmlElement("bfefrmtrm_dt")]
        public string bfefrmtrm_dt { get; set; }        //	전전기일자	Y	ex) 2016.12.31 현재(※ 사업보고서의 경우에만 출력)
        [XmlElement("bfefrmtrm_amount")]
        public string bfefrmtrm_amount { get; set; }        //	전전기금액	Y	9,999,999,999(※ 사업보고서의 경우에만 출력)
        [XmlElement("ord")]
        public string ord { get; set; }        //	계정과목 정렬순서	Y	계정과목 정렬순서

        public ResFnlttMultiAcntItem()
        {
            rcept_no = "";
            bsns_year = "";
            stock_code = "";
            reprt_code = "";
            account_nm = "";
            fs_div = "";
            fs_nm = "";
            sj_div = "";
            sj_nm = "";
            thstrm_nm = "";
            thstrm_dt = "";
            thstrm_amount = "";
            thstrm_add_amount = "";
            frmtrm_nm = "";
            frmtrm_dt = "";
            frmtrm_amount = "";
            frmtrm_add_amount = "";
            bfefrmtrm_nm = "";
            bfefrmtrm_dt = "";
            bfefrmtrm_amount = "";
            ord = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResFnlttMultiAcntItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("bsns_year: {0}", bsns_year);
            Console.WriteLine("stock_code: {0}", stock_code);
            Console.WriteLine("reprt_code: {0}", reprt_code);
            Console.WriteLine("account_nm: {0}", account_nm);
            Console.WriteLine("fs_div: {0}", fs_div);
            Console.WriteLine("fs_nm: {0}", fs_nm);
            Console.WriteLine("sj_div: {0}", sj_div);
            Console.WriteLine("sj_nm: {0}", sj_nm);
            Console.WriteLine("thstrm_nm: {0}", thstrm_nm);
            Console.WriteLine("thstrm_dt: {0}", thstrm_dt);
            Console.WriteLine("thstrm_amount: {0}", thstrm_amount);
            Console.WriteLine("thstrm_add_amount: {0}", thstrm_add_amount);
            Console.WriteLine("frmtrm_nm: {0}", frmtrm_nm);
            Console.WriteLine("frmtrm_dt: {0}", frmtrm_dt);
            Console.WriteLine("frmtrm_amount: {0}", frmtrm_amount);
            Console.WriteLine("frmtrm_add_amount: {0}", frmtrm_add_amount);
            Console.WriteLine("bfefrmtrm_nm: {0}", bfefrmtrm_nm);
            Console.WriteLine("bfefrmtrm_dt: {0}", bfefrmtrm_dt);
            Console.WriteLine("bfefrmtrm_amount: {0}", bfefrmtrm_amount);
            Console.WriteLine("ord: {0}", ord);
            Console.WriteLine("==================================================");
        }
    }
}