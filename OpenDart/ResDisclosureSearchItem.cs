using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResDisclosureSearchItem
    {
        [XmlElement("corp_name")]
        public string corp_name { get; set; }   // 종목명(법인명)	Y	공시대상회사의 종목명(상장사) 또는 법인명(기타법인)
        [XmlElement("corp_code")]
        public string corp_code { get; set; }   // 고유번호	Y	공시대상회사의 고유번호(8자리)
        [XmlElement("stock_code")]
        public string stock_code { get; set; }  // 종목코드	Y	상장회사의 종목코드(6자리)
        [XmlElement("corp_cls")]
        public string corp_cls { get; set; }    // 법인구분	Y	법인구분 : Y(유가), K(코스닥), N(코넥스), E(기타)
        [XmlElement("report_nm")]
        public string report_nm { get; set; }   // 보고서명	Y	공시구분+보고서명+기타정보
                                                // [기재정정] : 본 보고서명으로 이미 제출된 보고서의 기재내용이 변경되어 제출된 것임
                                                // [첨부정정] : 본 보고서명으로 이미 제출된 보고서의 첨부내용이 변경되어 제출된 것임
                                                // [첨부추가] : 본 보고서명으로 이미 제출된 보고서의 첨부서류가 추가되어 제출된 것임
                                                // [변경등록] : 본 보고서명으로 이미 제출된 보고서의 유동화계획이 변경되어 제출된 것임
                                                // [연장결정] : 본 보고서명으로 이미 제출된 보고서의 신탁계약이 연장되어 제출된 것임
                                                // [발행조건확정] : 본 보고서명으로 이미 제출된 보고서의 유가증권 발행조건이 확정되어 제출된 것임
                                                // [정정명령부과] : 본 보고서에 대하여 금융감독원이 정정명령을 부과한 것임
                                                // [정정제출요구] : 본 보고서에 대하여 금융감독원이 정정제출요구을 부과한 것임
        [XmlElement("rcept_no")]
        public string rcept_no { get; set; }    // 접수번호	Y	접수번호(14자리)
                                                // ※ 공시뷰어 연결에 이용예시
                                                // - PC용 : http://dart.fss.or.kr/dsaf001/main.do?rcpNo=접수번호
                                                // - 모바일용 : http://m.dart.fss.or.kr/html_mdart/MD1007.html?rcpNo=접수번호
        [XmlElement("flr_nm")]
        public string flr_nm { get; set; }      //	공시 제출인명	Y	공시 제출인명
        [XmlElement("rcept_dt")]
        public string rcept_dt { get; set; }    //	접수일자	Y	공시 접수일자(YYYYMMDD)
        [XmlElement("rm")]
        public string rm { get; set; }          // 비고	Y	조합된 문자로 각각은 아래와 같은 의미가 있음
                                                // 유 : 본 공시사항은 한국거래소 유가증권시장본부 소관임
                                                // 코 : 본 공시사항은 한국거래소 코스닥시장본부 소관임
                                                // 채 : 본 문서는 한국거래소 채권상장법인 공시사항임
                                                // 넥 : 본 문서는 한국거래소 코넥스시장 소관임
                                                // 공 : 본 공시사항은 공정거래위원회 소관임
                                                // 연 : 본 보고서는 연결부분을 포함한 것임
                                                // 정 : 본 보고서 제출 후 정정신고가 있으니 관련 보고서를 참조하시기 바람
                                                // 철 : 본 보고서는 철회(간주)되었으니 관련 철회신고서(철회간주안내)를 참고하시기 바람

        public ResDisclosureSearchItem()
        {
            corp_cls = "";
            corp_name = "";
            corp_code = "";
            stock_code = "";
            report_nm = "";
            rcept_no = "";
            flr_nm = "";
            rcept_dt = "";
            rm = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResDisclosureSearchItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("corp_name: {0}", corp_name);
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("stock_code: {0}", stock_code);
            Console.WriteLine("report_nm: {0}", report_nm);
            Console.WriteLine("rcept_no: {0}", rcept_no);
            Console.WriteLine("flr_nm: {0}", flr_nm);
            Console.WriteLine("rcept_dt: {0}", rcept_dt);
            Console.WriteLine("rm: {0}", rm);
            Console.WriteLine("==================================================");
        }
    }
}