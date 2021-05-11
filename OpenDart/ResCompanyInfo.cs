using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    [XmlRoot("result")]
    public class ResCompanyInfo
    {
        [XmlElement("status")]
        public string status { get; set; }          // 에러 및 정보 코드(※메시지 설명 참조)
        [XmlElement("message")]
        public string message { get; set; }         // 에러 및 정보 메시지(※메시지 설명 참조)
        [XmlElement("corp_name")]
        public string corp_name { get; set; }       // 정식명칭        정식회사명칭
        [XmlElement("corp_name_eng")]
        public string corp_name_eng { get; set; }   // 영문명칭 영문정식회사명칭
        [XmlElement("stock_name")]
        public string stock_name { get; set; }      // 종목명(상장사) 또는 약식명칭(기타법인)      종목명(상장사) 또는 약식명칭(기타법인)
        [XmlElement("stock_code")]
        public string stock_code { get; set; }      // 상장회사인 경우 주식의 종목코드 상장회사의 종목코드(6자리)
        [XmlElement("ceo_nm")]
        public string ceo_nm { get; set; }          // 대표자명        대표자명
        [XmlElement("corp_cls")]
        public string corp_cls { get; set; }        //    법인구분 법인구분 : Y(유가), K(코스닥), N(코넥스), E(기타)
        [XmlElement("jurir_no")]
        public string jurir_no { get; set; }        // 법인등록번호      법인등록번호
        [XmlElement("bizr_no")]
        public string bizr_no { get; set; }         // 사업자등록번호 사업자등록번호
        [XmlElement("adres")]
        public string adres { get; set; }           // 주소      주소
        [XmlElement("hm_url")]
        public string hm_url { get; set; }          // 홈페이지 홈페이지
        [XmlElement("ir_url")]
        public string ir_url { get; set; }          // IR홈페이지      IR홈페이지
        [XmlElement("phn_no")]
        public string phn_no { get; set; }          //  전화번호 전화번호
        [XmlElement("fax_no")]
        public string fax_no { get; set; }          // 팩스번호        팩스번호
        [XmlElement("induty_code")]
        public string induty_code { get; set; }     // 업종코드 업종코드
        [XmlElement("est_dt")]
        public string est_dt { get; set; }          // 설립일(YYYYMMDD)       설립일(YYYYMMDD)
        [XmlElement("acc_mt")]
        public string acc_mt { get; set; }          // 결산월(MM)     결산월(MM)

        public ResCompanyInfo()
        {
            status        = "";
            message       = "";
            corp_name     = "";
            corp_name_eng = "";
            stock_name    = "";
            stock_code    = "";
            ceo_nm        = "";
            corp_cls      = "";
            jurir_no      = "";
            bizr_no       = "";
            adres         = "";
            hm_url        = "";
            ir_url        = "";
            phn_no        = "";
            fax_no        = "";
            induty_code   = "";
            est_dt        = "";
            acc_mt        = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResCompanyInfo Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("status: {0}", status);
            Console.WriteLine("message: {0}", message);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("corp_name_eng: {0}", corp_name_eng);
            Console.WriteLine("stock_name: {0}", stock_name);
            Console.WriteLine("stock_code: {0}", stock_code);
            Console.WriteLine("ceo_nm: {0}", ceo_nm);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("bizr_no: {0}", bizr_no);
            Console.WriteLine("adres: {0}", adres);
            Console.WriteLine("hm_url: {0}", hm_url);
            Console.WriteLine("ir_url: {0}", ir_url);
            Console.WriteLine("phn_no: {0}", phn_no);
            Console.WriteLine("fax_no: {0}", fax_no);
            Console.WriteLine("induty_code: {0}", induty_code);
            Console.WriteLine("est_dt: {0}", est_dt);
            Console.WriteLine("acc_mt: {0}", acc_mt);
            Console.WriteLine("==================================================");
        }
    }
}