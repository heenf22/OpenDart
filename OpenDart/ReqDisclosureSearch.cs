using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ReqDisclosureSearch
    {
        public string corp_code { get; set; }       // 고유번호	STRING(8)	N	공시대상회사의 고유번호(8자리)
                                                    // ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
        public string bgn_de { get; set; }          // 시작일	STRING(8)	N	검색시작 접수일자(YYYYMMDD) : 없으면 종료일(end_de)
                                                    // 고유번호(corp_code)가 없는 경우 검색기간은 3개월로 제한
        public string end_de { get; set; }          // 종료일	STRING(8)	N	검색종료 접수일자(YYYYMMDD) : 없으면 당일
        public string last_reprt_at { get; set; }   // 최종보고서 검색여부	STRING(1)	N	최종보고서만 검색여부(Y or N) 기본값 : N
                                                    // (정정이 있는 경우 최종정정만 검색)
        public string pblntf_ty { get; set; }       // 공시유형	STRING(1)	N	A : 정기공시
                                                    // B : 주요사항보고
                                                    // C : 발행공시
                                                    // D : 지분공시
                                                    // E : 기타공시
                                                    // F : 외부감사관련
                                                    // G : 펀드공시
                                                    // H : 자산유동화
                                                    // I : 거래소공시
                                                    // j : 공정위공시
        public string pblntf_detail_ty { get; set; }    // 공시상세유형	STRING(4)	N	(※ 상세 유형 참조 : pblntf_detail_ty)
        public string corp_cls { get; set; }        // 법인구분	STRING(1)	N	법인구분 : Y(유가), K(코스닥), N(코넥스), E(기타)
                                                    // ※ 없으면 전체조회, 복수조건 불가
        public string sort { get; set; }            // 정렬	STRING(4)	N	접수일자: date
                                                    // 회사명 : crp
                                                    // 보고서명 : rpt
                                                    // 기본값 : date
        public string sort_mth { get; set; }        // 정렬방법	STRING(4)	N	오름차순(asc), 내림차순(desc) 기본값 : desc
        public string page_no { get; set; }         // 페이지 번호	STRING(5)	N	페이지 번호(1~n) 기본값 : 1
        public string page_count { get; set; }      // 페이지 별 건수	STRING(3)	N	페이지당 건수(1~100) 기본값 : 10, 최대값 : 100

        public ReqDisclosureSearch()
        {
            corp_code = "";
            bgn_de = DateTime.Now.ToString("yyyyMMdd");
            end_de = DateTime.Now.ToString("yyyyMMdd");
            last_reprt_at = "";
            pblntf_ty = "";
            pblntf_detail_ty = "";
            corp_cls = "Y";
            sort = "";
            sort_mth = "";
            page_no = "1";
            page_count = "10";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("CompanyInfo Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("corp_code: {0}", corp_code);
            Console.WriteLine("bgn_de: {0}", bgn_de);
            Console.WriteLine("end_de: {0}", end_de);
            Console.WriteLine("last_reprt_at: {0}", last_reprt_at);
            Console.WriteLine("pblntf_ty: {0}", pblntf_ty);
            Console.WriteLine("pblntf_detail_ty: {0}", pblntf_detail_ty);
            Console.WriteLine("corp_cls: {0}", corp_cls);
            Console.WriteLine("sort: {0}", sort);
            Console.WriteLine("sort_mth: {0}", sort_mth);
            Console.WriteLine("page_no: {0}", page_no);
            Console.WriteLine("page_count: {0}", page_count);
            Console.WriteLine("==================================================");
        }
    }
}