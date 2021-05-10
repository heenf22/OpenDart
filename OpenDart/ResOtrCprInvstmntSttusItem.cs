using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResOtrCprInvstmntSttusItem
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
        public string inv_prm { get; set; }       //	법인명	Y	법인명
        public string frst_acqs_de { get; set; }       //	최초 취득 일자	Y	최초취득일자(YYYYMMDD)
        public string invstmnt_purps { get; set; }       //	출자 목적	Y	출자목적(자회사 등)
        public string frst_acqs_amount { get; set; }       //	최초 취득 금액	Y	9,999,999,999
        public string bsis_blce_qy { get; set; }       //	기초 잔액 수량	Y	9,999,999,999
        public string bsis_blce_qota_rt { get; set; }       //	기초 잔액 지분 율	Y	0.00
        public string bsis_blce_acntbk_amount { get; set; }       //	기초 잔액 장부 가액	Y	9,999,999,999
        public string incrs_dcrs_acqs_dsps_qy { get; set; }       //	증가 감소 취득 처분 수량	Y	9,999,999,999
        public string incrs_dcrs_acqs_dsps_amount { get; set; }       //	증가 감소 취득 처분 금액	Y	9,999,999,999
        public string incrs_dcrs_evl_lstmn { get; set; }       //	증가 감소 평가 손액	Y	9,999,999,999
        public string trmend_blce_qy { get; set; }       //	기말 잔액 수량	Y	9,999,999,999
        public string trmend_blce_qota_rt { get; set; }       //	기말 잔액 지분 율	Y	0.00
        public string trmend_blce_acntbk_amount { get; set; }       //	기말 잔액 장부 가액	Y	9,999,999,999
        public string recent_bsns_year_fnnr_sttus_tot_assets { get; set; }       //	최근 사업 연도 재무 현황 총 자산	Y	9,999,999,999
        public string recent_bsns_year_fnnr_sttus_thstrm_ntpf { get; set; }       //	최근 사업 연도 재무 현황 당기 순이익	Y	9,999,999,999

        public ResOtrCprInvstmntSttusItem()
        {
            rcept_no = "";
            corp_cls = "";
            corp_code = "";
            corp_name = "";
            inv_prm = "";
            frst_acqs_de = "";
            invstmnt_purps = "";
            frst_acqs_amount = "";
            bsis_blce_qy = "";
            bsis_blce_qota_rt = "";
            bsis_blce_acntbk_amount = "";
            incrs_dcrs_acqs_dsps_qy = "";
            incrs_dcrs_acqs_dsps_amount = "";
            incrs_dcrs_evl_lstmn = "";
            trmend_blce_qy = "";
            trmend_blce_qota_rt = "";
            trmend_blce_acntbk_amount = "";
            recent_bsns_year_fnnr_sttus_tot_assets = "";
            recent_bsns_year_fnnr_sttus_thstrm_ntpf = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResOtrCprInvstmntSttusItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("inv_prm: {0}", inv_prm);
            Console.WriteLine("frst_acqs_de: {0}", frst_acqs_de);
            Console.WriteLine("invstmnt_purps: {0}", invstmnt_purps);
            Console.WriteLine("frst_acqs_amount: {0}", frst_acqs_amount);
            Console.WriteLine("bsis_blce_qy: {0}", bsis_blce_qy);
            Console.WriteLine("bsis_blce_qota_rt: {0}", bsis_blce_qota_rt);
            Console.WriteLine("bsis_blce_acntbk_amount: {0}", bsis_blce_acntbk_amount);
            Console.WriteLine("incrs_dcrs_acqs_dsps_qy: {0}", incrs_dcrs_acqs_dsps_qy);
            Console.WriteLine("incrs_dcrs_acqs_dsps_amount: {0}", incrs_dcrs_acqs_dsps_amount);
            Console.WriteLine("incrs_dcrs_evl_lstmn: {0}", incrs_dcrs_evl_lstmn);
            Console.WriteLine("trmend_blce_qy: {0}", trmend_blce_qy);
            Console.WriteLine("trmend_blce_qota_rt: {0}", trmend_blce_qota_rt);
            Console.WriteLine("trmend_blce_acntbk_amount: {0}", trmend_blce_acntbk_amount);
            Console.WriteLine("recent_bsns_year_fnnr_sttus_tot_assets: {0}", recent_bsns_year_fnnr_sttus_tot_assets);
            Console.WriteLine("recent_bsns_year_fnnr_sttus_thstrm_ntpf: {0}", recent_bsns_year_fnnr_sttus_thstrm_ntpf);
            Console.WriteLine("==================================================");
        }
    }
}