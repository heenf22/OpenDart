using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResMajorstockItem
    {
        [XmlElement("rcept_no")]
        public string rcept_no { get; set; }        // 접수번호	Y	접수번호(14자리)
                                                    // ※ 공시뷰어 연결에 이용예시
                                                    // - PC용 : http://dart.fss.or.kr/dsaf001/main.do?rcpNo=접수번호
                                                    // - 모바일용 : http://m.dart.fss.or.kr/html_mdart/MD1007.html?rcpNo=접수번호
        [XmlElement("rcept_dt")]
        public string rcept_dt { get; set; }        //	접수일자	Y	공시 접수일자(YYYYMMDD)
        [XmlElement("stock_code")]
        public string stock_code { get; set; }      //	종목코드	Y	상장회사의 종목코드(6자리)
        [XmlElement("cmpny_nm")]
        public string cmpny_nm { get; set; }        //	회사명	Y	공시대상회사의 종목명(상장사) 또는 법인명(기타법인)
        [XmlElement("report_tp")]
        public string report_tp { get; set; }       //	보고구분	Y	주식등의 대량보유상황 보고구분
        [XmlElement("repror")]
        public string repror { get; set; }          //	대표보고자	Y	대표보고자
        [XmlElement("stkqy")]
        public string stkqy { get; set; }           //	보유주식등의 수	Y	보유주식등의 수
        [XmlElement("stkqy_irds")]
        public string stkqy_irds { get; set; }      //	보유주식등의 증감	Y	보유주식등의 증감
        [XmlElement("stkrt")]
        public string stkrt { get; set; }           //	보유비율	Y	보유비율
        [XmlElement("stkrt_irds")]
        public string stkrt_irds { get; set; }      //	보유비율 증감	Y	보유비율 증감
        [XmlElement("ctr_stkqy")]
        public string ctr_stkqy { get; set; }       //	주요체결 주식등의 수	Y	주요체결 주식등의 수
        [XmlElement("ctr_stkrt")]
        public string ctr_stkrt { get; set; }       //	주요체결 보유비율	Y	주요체결 보유비율
        [XmlElement("report_resn")]
        public string report_resn { get; set; }     //	보고사유	Y	보고사유

        public ResMajorstockItem()
        {
            rcept_no = "";
            rcept_dt = "";
            stock_code = "";
            cmpny_nm = "";
            report_tp = "";
            repror = "";
            stkqy = "";
            stkqy_irds = "";
            stkrt = "";
            stkrt_irds = "";
            ctr_stkqy = "";
            ctr_stkrt = "";
            report_resn = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResMajorstockItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("rcept_dt: {0}", rcept_dt);
            Console.WriteLine("stock_code: {0}", stock_code);
            Console.WriteLine("cmpny_nm: {0}", cmpny_nm);
            Console.WriteLine("report_tp: {0}", report_tp);
            Console.WriteLine("repror: {0}", repror);
            Console.WriteLine("stkqy: {0}", stkqy);
            Console.WriteLine("stkqy_irds: {0}", stkqy_irds);
            Console.WriteLine("stkrt: {0}", stkrt);
            Console.WriteLine("stkrt_irds: {0}", stkrt_irds);
            Console.WriteLine("ctr_stkqy: {0}", ctr_stkqy);
            Console.WriteLine("ctr_stkrt: {0}", ctr_stkrt);
            Console.WriteLine("report_resn: {0}", report_resn);
            Console.WriteLine("==================================================");
        }
    }
}