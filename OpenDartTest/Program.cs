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
            // 공시정보 테스트 (REQ1_XXX)
            //========================================================================
            // 공시검색: 공시 유형별, 회사별, 날짜별 등 여러가지 조건으로 공시보고서 검색기능을 제공합니다.
            // ReqDisclosureSearch rds = new ReqDisclosureSearch();
            // OpenDartClient.Instance.REQ1_1_GET_DISCLOSURE_SEARCH(rds);

            // 기업개황: 두산중공업(00159616)
            // OpenDartClient.Instance.REQ1_2_GET_COMPANY_INFO("00159616", true);

            // 공시서류원본파일: 20190401004781
            // OpenDartClient.Instance.REQ1_3_GET_DOCUMENT("20190401004781");

            // 고유번호(전체 기업 종목코드 파일 다운로드 및 설정)
            // OpenDartClient.Instance.REQ1_4_GET_CORPCODE();
            //========================================================================

            //========================================================================
            // 사업보고서 주요정보 테스트 (REQ2_XXX)
            //========================================================================
            // 증자(감자) 현황, corp_code=00126380&bsns_year=2018&reprt_code=11011
            // OpenDartClient.Instance.REQ2_1_GET_IRDS_STTUS_INFO("00126380", "2018", "11011");

            // 배당에 관한 사항, corp_code=00126380&bsns_year=2018&reprt_code=11011
            OpenDartClient.Instance.REQ2_2_GET_ALOT_MATTER_INFO("00126380", "2018", "11011");

            //========================================================================
        }
    }
}
