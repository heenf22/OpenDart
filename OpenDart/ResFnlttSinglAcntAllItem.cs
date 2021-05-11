using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResFnlttSinglAcntAllItem
    {
        [XmlElement("rcept_no")]
        public string rcept_no { get; set; }            // 접수번호	Y	접수번호(14자리)
                                                        // ※ 공시뷰어 연결에 이용예시
                                                        // - PC용 : http://dart.fss.or.kr/dsaf001/main.do?rcpNo=접수번호
                                                        // - 모바일용 : http://m.dart.fss.or.kr/html_mdart/MD1007.html?rcpNo=접수번호
        [XmlElement("reprt_code")]
        public string reprt_code { get; set; }          //	보고서 코드	Y	1분기보고서 : 11013
                                                        // 반기보고서 : 11012
                                                        // 3분기보고서 : 11014
                                                        // 사업보고서 : 11011
        [XmlElement("bsns_year")]
        public string bsns_year { get; set; }           //	사업 연도	Y	2019
        [XmlElement("corp_code")]
        public string corp_code { get; set; }           //	고유번호	Y	공시대상회사의 고유번호(8자리)
        [XmlElement("sj_div")]
        public string sj_div { get; set; }              //	재무제표구분	Y	BS : 재무상태표
                                                        // IS : 손익계산서
                                                        // CIS : 포괄손익계산서
                                                        // CF : 현금흐름표
                                                        // SCE : 자본변동표
        [XmlElement("sj_nm")]
        public string sj_nm { get; set; }               //	재무제표명	Y	ex) 재무상태표 또는 손익계산서 출력
        [XmlElement("account_id")]
        public string account_id { get; set; }          //	계정ID	Y	XBRL 표준계정ID
                                                        // ※ 표준계정ID가 아닐경우 "-표준계정코드 미사용-" 표시
        [XmlElement("account_nm")]
        public string account_nm { get; set; }          //	계정명	Y	계정명칭
                                                        // ex) 자본총계
        [XmlElement("account_detail")]
        public string account_detail { get; set; }      //	계정상세	Y	※ 자본변동표에만 출력
                                                        // ex) 계정 상세명칭 예시
                                                        // - 자본 [member]|지배기업 소유주지분
                                                        // - 자본 [member]|지배기업 소유주지분|기타포괄손익누계액 [member]
        [XmlElement("thstrm_nm")]
        public string thstrm_nm { get; set; }           //	당기명	Y	ex) 제 13 기
        [XmlElement("thstrm_amount")]
        public string thstrm_amount { get; set; }       //	당기금액	Y	9,999,999,999
                                                        // ※ 분/반기 보고서이면서 (포괄)손익계산서 일 경우 [3개월] 금액
        [XmlElement("thstrm_add_amount")]
        public string thstrm_add_amount { get; set; }   //	당기누적금액	Y	9,999,999,999
        [XmlElement("frmtrm_nm")]
        public string frmtrm_nm { get; set; }           //	전기명	Y	ex) 제 12 기말
        [XmlElement("frmtrm_amount")]
        public string frmtrm_amount { get; set; }       //	전기금액	Y	9,999,999,999
        [XmlElement("frmtrm_q_nm")]
        public string frmtrm_q_nm { get; set; }         //	전기명(분/반기)	Y	ex) 제 18 기 반기
        [XmlElement("frmtrm_q_amount")]
        public string frmtrm_q_amount { get; set; }     //	전기금액(분/반기)	Y	9,999,999,999
                                                        // ※ 분/반기 보고서이면서 (포괄)손익계산서 일 경우 [3개월] 금액
        [XmlElement("frmtrm_add_amount")]
        public string frmtrm_add_amount { get; set; }   //	전기누적금액	Y	9,999,999,999
        [XmlElement("bfefrmtrm_nm")]
        public string bfefrmtrm_nm { get; set; }        //	전전기명	Y	ex) 제 11 기말(※ 사업보고서의 경우에만 출력)
        [XmlElement("bfefrmtrm_amount")]
        public string bfefrmtrm_amount { get; set; }    //	전전기금액	Y	9,999,999,999(※ 사업보고서의 경우에만 출력)
        [XmlElement("ord")]
        public string ord { get; set; }                 //	계정과목 정렬순서	Y	계정과목 정렬순서

        public ResFnlttSinglAcntAllItem()
        {
            rcept_no = "";
            reprt_code = "";
            bsns_year = "";
            corp_code = "";
            sj_div = "";
            sj_nm = "";
            account_id = "";
            account_nm = "";
            account_detail = "";
            thstrm_nm = "";
            thstrm_amount = "";
            thstrm_add_amount = "";
            frmtrm_nm = "";
            frmtrm_amount = "";
            frmtrm_q_nm = "";
            frmtrm_q_amount = "";
            frmtrm_add_amount = "";
            bfefrmtrm_nm = "";
            bfefrmtrm_amount = "";
            ord = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResFnlttSinglAcntAllItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("reprt_code: {0}", reprt_code);
            Console.WriteLine("bsns_year: {0}", bsns_year);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("sj_div: {0}", sj_div);
            Console.WriteLine("sj_nm: {0}", sj_nm);
            Console.WriteLine("account_id: {0}", account_id);
            Console.WriteLine("account_nm: {0}", account_nm);
            Console.WriteLine("account_detail: {0}", account_detail);
            Console.WriteLine("thstrm_nm: {0}", thstrm_nm);
            Console.WriteLine("thstrm_amount: {0}", thstrm_amount);
            Console.WriteLine("thstrm_add_amount: {0}", thstrm_add_amount);
            Console.WriteLine("frmtrm_nm: {0}", frmtrm_nm);
            Console.WriteLine("frmtrm_amount: {0}", frmtrm_amount);
            Console.WriteLine("frmtrm_q_nm: {0}", frmtrm_q_nm);
            Console.WriteLine("frmtrm_q_amount: {0}", frmtrm_q_amount);
            Console.WriteLine("frmtrm_add_amount: {0}", frmtrm_add_amount);
            Console.WriteLine("bfefrmtrm_nm: {0}", bfefrmtrm_nm);
            Console.WriteLine("bfefrmtrm_amount: {0}", bfefrmtrm_amount);
            Console.WriteLine("ord: {0}", ord);
            Console.WriteLine("==================================================");
        }
    }
}