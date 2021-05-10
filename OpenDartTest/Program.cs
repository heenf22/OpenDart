using System;
using OpenDart.Models;
using OpenDart.OpenDartClient;

namespace OpenDartTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello TestOpenDart!");

            // Open DART API Key(https://opendart.fss.or.kr/ 에서 발급받아야함)
            OpenDartClient.Instance.apiKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            OpenDartClient.Instance.dummyDirectory = @"/home/lgh/project/public/dummy";

            //========================================================================
            // 1. 공시정보 테스트 (REQ1_XXX)
            //========================================================================
            // 1.1. 공시검색: 공시 유형별, 회사별, 날짜별 등 여러가지 조건으로 공시보고서 검색기능을 제공합니다.
            // ReqDisclosureSearch rds = new ReqDisclosureSearch();
            // OpenDartClient.Instance.REQ1_1_GET_DISCLOSURE_SEARCH(rds);

            // 1.2. 기업개황: 두산중공업(00159616)
            // OpenDartClient.Instance.REQ1_2_GET_COMPANY_INFO("00159616", true);

            // 1.3. 공시서류원본파일: 20190401004781
            // OpenDartClient.Instance.REQ1_3_GET_DOCUMENT("20190401004781");

            // 1.4. 고유번호(전체 기업 종목코드 파일 다운로드 및 설정)
            // OpenDartClient.Instance.REQ1_4_GET_CORPCODE();
            //========================================================================

            //========================================================================
            // 2. 사업보고서 주요정보 테스트 (REQ2_XXX)
            //========================================================================
            // 2.1. 증자(감자) 현황, corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_1_GET_IRDS_STTUS_INFO("00126380", "2018", "11011");

            // 2.2. 배당에 관한 사항, corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_2_GET_ALOT_MATTER_INFO("00126380", "2018", "11011");

            // 2.3. 자기주식 취득 및 처분 현황, corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_3_GET_TESSTK_ACQS_DSPS_STTUS_INFO("00126380", "2018", "11011");

            // 2.4. 최대주주 현황, corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_4_GET_HYSLR_STTUS_INFO("00126380", "2018", "11011");

            // 2.5. 최대주주 변동 현황, corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_5_GET_HYSLR_CHG_STTUS_INFO("00126380", "2018", "11011");

            // 2.6. 소액주주현황, corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_6_GET_MRHL_STTUS_INFO("00126380", "2018", "11011");

            // 2.7. 임원현황, corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_7_GET_EXCTV_STTUS_INFO("00126380", "2018", "11011");

            // 2.8. 직원현황, corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_8_GET_EMP_STTUS_INFO("00126380", "2018", "11011");

            // 2.9. 이사ㆍ감사의 개인별 보수 현황, corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_9_GET_HMV_AUDIT_INDVDL_BY_STTUS_INFO("00126380", "2018", "11011");

            // 2.10. 이사ㆍ감사 전체의 보수현황, corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_10_GET_HMV_AUDIT_ALL_STTUS_INFO("00126380", "2018", "11011");

            // 2.11. 개인별 보수지급 금액(5억이상 상위5인), corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_11_GET_INDVDL_BY_PAY_INFO("00126380", "2018", "11011");

            // 2.12. 타법인 출자현황, corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_12_GET_OTR_CPR_INVSTMNT_STTUS_INFO("00126380", "2018", "11011");
            //========================================================================

            //========================================================================
            // 3. 상장기업 재무정보 테스트 (REQ3_XXX)
            //========================================================================
            // 3.1. 단일회사 주요계정, corp_code=00126380&bsns_year=2018&reprt_code=11011
            OpenDartClient.Instance.REQ3_1_GET_FNLTT_SINGL_ACNT_INFO("00126380", "2018", "11011");
            //========================================================================
        }
    }
}
