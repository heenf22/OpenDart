using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using OpenDart.Models;

// #pragma warning disable SYSLIB0014

namespace OpenDart.OpenDartClient
{
    // 통신 결과 정보
    public struct REQ_RESULT_STATUS
    {
        public string PROTOCOL_NAME;        // 수행 프로토콜 이름
        public int BEGIN_TICKCOUNT;         // 프로토콜 시작 Tickcount
        public int END_TICKCOUNT;           // 프로코콜 종료 Tickcount
        public string CODE;                 // 결과 코드(0:Susses, ...)
        public string MESSAGE;              // 결과 메시지
        public string EXCEPTION;            // Exception Message
    }

    public class OpenDartClient
    {
        public string dummyDirectory { get; set; }
        public int timeOut { get; set; } = 300;     // sec
        public bool useProxy { get; set; } = false;
        public string proxyIp { get; set; } = "127.0.0.1";
        public int proxyPort { get; set; } = 8080;
        public bool isLogin { get; set; } = false;
        public REQ_RESULT_STATUS reqResultStatus;

        // Open DART 쿼리용 키로 회원가입 후 발급받아 사용, 하루 이용횟수에 제한이 있음
        public string apiKey { get; set; }
        public string apiUri { get; set; } = "https://opendart.fss.or.kr/api";
        public int requestApiKeyCount { get; set; }

        public OpenDartClient()
        {
            apiKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            dummyDirectory = @"C:\Users\heenf\Desktop\Project\dummy";
            initialize();
        }

        public OpenDartClient(string apiKey, string dummyPath)
        {
            this.apiKey = apiKey;
            dummyDirectory = dummyPath;
            // dummyDirectory = @"C:\Users\heenf\Desktop\Project\dummy";
            initialize();
        }

        private void initialize()
        {
            requestApiKeyCount = 0;

            // X.509 SSL Define (private OCP) SSL 통신을 위해 CertificatePolicy property 등록
            // System.Net.ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            // or
            ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |      // TLS 1.0
            //                                        SecurityProtocolType.Tls11 |    // TLS 1.1
            //                                        SecurityProtocolType.Tls12 |    // TLS 1.2   
            //                                        SecurityProtocolType.Tls13;     // TLS 1.3
        }

        // X.509 SSL Define (private OCP)
        //public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //{
        //    return result;
        //}

        private CookieCollection cookiecollection;

        public CookieCollection getCooki()
        {
            return cookiecollection;
        }

        public void setCooki(CookieCollection cc)
        {
            cookiecollection = cc;
        }

        public string HexDump(string message, int bytesPerLine = 16)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            return HexDump(bytes, bytesPerLine);
        }

        public string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 characters for the address
                + 1;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }

                    hexColumn += 3;
                    charColumn++;
                }

                result.Append(line);
            }

            return result.ToString();
        }

        private void debugBeginProtocol(string protocolName)
        {
            reqResultStatus.EXCEPTION = "";
            reqResultStatus.BEGIN_TICKCOUNT = Environment.TickCount;
            reqResultStatus.PROTOCOL_NAME = protocolName;
            Console.WriteLine("");
            Console.WriteLine("---- Begin {0}: {1}", reqResultStatus.PROTOCOL_NAME, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        }

        private void debugEndProtocol()
        {
            reqResultStatus.END_TICKCOUNT = Environment.TickCount;
            Console.WriteLine("---- End {0}: {1} <-- {2} [ms]", reqResultStatus.PROTOCOL_NAME, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), (reqResultStatus.END_TICKCOUNT - reqResultStatus.BEGIN_TICKCOUNT).ToString("#,##0"));
        }

        private void DebugRequest(HttpWebRequest request, byte[] reqData, bool isText = false)
        {
            Console.WriteLine("\n[Request Header]");
            Console.WriteLine("URI: {0} {1} {2}", request.Method, request.ProtocolVersion, request.Address);
            Console.WriteLine("Timeout: " + request.Timeout + " [msec]");
            Console.WriteLine("ReadWriteTimeout: " + request.ReadWriteTimeout + " [msec]");
            Console.WriteLine("Accept: " + request.Accept);
            Console.WriteLine("Host: " + request.Host);
            Console.WriteLine("Content-Type: " + request.Headers["Content-Type"]);
            Console.WriteLine("Content-Length: " + request.ContentLength);
            // Console.WriteLine("X-Client-Key: " + request.Headers["X-Client-Key"]);
            Console.WriteLine("\n[Request Data: {0:#,##0} bytes]", reqData.Length);
            if (isText)
            {
                Console.WriteLine("{0}", Encoding.UTF8.GetString(reqData));
            }
            else
            {
                Console.WriteLine(HexDump(reqData));
            }
        }

        private void DebugResponse(HttpWebResponse response, byte[] resData, bool isText = false)
        {
            Console.WriteLine("\n[Response Header] " + (int)response.StatusCode + " " + response.StatusDescription);
            Console.WriteLine("URI: {0} {1} {2}", response.Method, response.ProtocolVersion, response.ResponseUri);
            Console.WriteLine("Server: " + response.Server);
            Console.WriteLine("Character-Set: " + response.CharacterSet);
            Console.WriteLine("Content-Encoding: " + response.ContentEncoding);
            Console.WriteLine("Content-Type: " + response.Headers["Content-Type"]);
            Console.WriteLine("Content-Length: " + response.ContentLength);
            // Console.WriteLine("X-Client-Key: " + response.Headers["X-Client-Key"]);
            Console.WriteLine("\n[Response Data: {0:#,##0} bytes]", resData.Length);
            if (isText)
            {
                Console.WriteLine("{0}", Encoding.UTF8.GetString(resData));
            }
            else
            {
                Console.WriteLine(HexDump(resData));
            }
        }

        private void displayWebException(WebException e)
        {
            Console.WriteLine("*******************************************************************************");
            Console.WriteLine("!!! Web EXCEPTION: " + e.Message);
            HttpWebResponse response = (HttpWebResponse)e.Response;
            MemoryStream ms = new MemoryStream();
            response.GetResponseStream().CopyTo(ms);
            byte[] resData = ms.ToArray();
            DebugResponse(response, resData);
            reqResultStatus.EXCEPTION += "\n";
            // iip_Status.EXCEPTION += Utility.Instance.byteToString(resData, Encoding.UTF8);
            ms.Close();
            response.Close();
            Console.WriteLine("*******************************************************************************");
        }

        private void checkApiKey()
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Could not find the api key. Please set the api key.");
                throw new Exception("Could not find the api key. Please set the api key.");
            }
        }

        //=====================================================================================================================================================
        //=====================================================================================================================================================
        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        // PROTOCOL - START
        // [공통 사항]
        // Response Status:
        //   - 000 :정상
        //   - 010 :등록되지 않은 키입니다.
        //   - 011 :사용할 수 없는 키입니다. 오픈API에 등록되었으나, 일시적으로 사용 중지된 키를 통하여 검색하는 경우 발생합니다.
        //   - 020 :요청 제한을 초과하였습니다.
        //          일반적으로는 10,000건 이상의 요청에 대하여 이 에러 메시지가 발생되나, 요청 제한이 다르게 설정된 경우에는 이에 준하여 발생됩니다.
        //   - 100 :필드의 부적절한 값입니다.필드 설명에 없는 값을 사용한 경우에 발생하는 메시지입니다.
        //   - 800 :원활한 공시서비스를 위하여 오픈API 서비스가 중지 중입니다.
        //   - 900 :정의되지 않은 오류가 발생하였습니다.
        //-----------------------------------------------------------------------------------------------------------------------------------------------------


        /******************************************************************************************************************************************************
         * Api Category : 1. 공시정보
         * Api Name     : 1.1. 공시검색, https://opendart.fss.or.kr/guide/main.do?apiGrpCd=DS001
         * Description  : 공시 유형별, 회사별, 날짜별 등 여러가지 조건으로 공시보고서 검색기능을 제공합니다.
         *              
         * Request URL: https://opendart.fss.or.kr/api/list.json
         *              https://opendart.fss.or.kr/api/list.xml
         * Request Parameter:
         * 키               명칭	        타입	        필수여부	    값설명
         * crtfc_key        API 인증키	    STRING(40)	Y	        발급받은 인증키(40자리)
         * corp_code	    고유번호	    STRING(8)	N	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bgn_de	        시작일	        STRING(8)	N	        검색시작 접수일자(YYYYMMDD) : 없으면 종료일(end_de)
         *                                                          고유번호(corp_code)가 없는 경우 검색기간은 3개월로 제한
         * end_de	        종료일	        STRING(8)	N	        검색종료 접수일자(YYYYMMDD) : 없으면 당일
         * last_reprt_at    최종보고서 검색여부	STRING(1)	N	        최종보고서만 검색여부(Y or N) 기본값 : N
         *                                                          (정정이 있는 경우 최종정정만 검색)
         * pblntf_ty	    공시유형	    STRING(1)	N	          A : 정기공시
         *                                                          B : 주요사항보고
         *                                                          C : 발행공시
         *                                                          D : 지분공시
         *                                                          E : 기타공시
         *                                                          F : 외부감사관련
         *                                                          G : 펀드공시
         *                                                          H : 자산유동화
         *                                                          I : 거래소공시
         *                                                          j : 공정위공시
         * pblntf_detail_ty	공시상세유형        STRING(4)	N	    (※ 상세 유형 참조 : pblntf_detail_ty)
         * corp_cls	    법인구분	           STRING(1)	N	      법인구분 : Y(유가), K(코스닥), N(코넥스), E(기타)
         *                                                          ※ 없으면 전체조회, 복수조건 불가
         * sort	        정렬	              STRING(4)	N	         접수일자: date
         *                                                          회사명 : crp
         *                                                          보고서명 : rpt
         *                                                          기본값 : date
         * sort_mth	    정렬방법	           STRING(4)	N	      오름차순(asc), 내림차순(desc) 기본값 : desc
         * page_no	    페이지 번호	           STRING(5)	N	       페이지 번호(1~n) 기본값 : 1
         * page_count	페이지 별 건수	        STRING(3)	N	        페이지당 건수(1~100) 기본값 : 10, 최대값 : 100
         * 
         * 상세유형:
         * pblntf_ty    pblntf_detail_ty    설명
         * A            A001                사업보고서
         *              A002	            반기보고서
         *              A003	            분기보고서
         *              A004	            등록법인결산서류(자본시장법이전)
         *              A005	            소액공모법인결산서류
         * B	        B001	            주요사항보고서
         *              B002	            주요경영사항신고(자본시장법 이전)
         *              B003	            최대주주등과의거래신고(자본시장법 이전)
         * C            C001	            증권신고(지분증권)
         *              C002	            증권신고(채무증권)
         *              C003	            증권신고(파생결합증권)
         *              C004	            증권신고(합병등)
         *              C005	            증권신고(기타)
         *              C006	            소액공모(지분증권)
         *              C007	            소액공모(채무증권)
         *              C008	            소액공모(파생결합증권)
         *              C009	            소액공모(합병등)
         *              C010	            소액공모(기타)
         *              C011	            호가중개시스템을통한소액매출
         * D	        D001	            주식등의대량보유상황보고서
         *              D002	            임원ㆍ주요주주특정증권등소유상황보고서
         *              D003	            의결권대리행사권유
         *              D004	            공개매수
         * E	        E001	            자기주식취득/처분
         *              E002	            신탁계약체결/해지
         *              E003	            합병등종료보고서
         *              E004	            주식매수선택권부여에관한신고
         *              E005	            사외이사에관한신고
         *              E006	            주주총회소집공고
         *              E007	            시장조성/안정조작
         *              E008	            합병등신고서(자본시장법 이전)
         *              E009	            금융위등록/취소(자본시장법 이전)
         * F	        F001	            감사보고서
         *              F002	            연결감사보고서
         *              F003	            결합감사보고서
         *              F004	            회계법인사업보고서
         *              F005	            감사전재무제표미제출신고서
         * G	        G001	            증권신고(집합투자증권-신탁형)
         *              G002	            증권신고(집합투자증권-회사형)
         *              G003	            증권신고(집합투자증권-합병)
         * H	        H001	            자산유동화계획/양도등록
         *              H002	            사업/반기/분기보고서
         *              H003	            증권신고(유동화증권등)
         *              H004	            채권유동화계획/양도등록
         *              H005	            수시보고
         *              H006	            주요사항보고서
         * I	        I001	            수시공시
         *              I002	            공정공시
         *              I003	            시장조치/안내
         *              I004	            지분공시
         *              I005	            증권투자회사
         *              I006	            채권공시
         * J	        J001	            대규모내부거래관련
         *              J002	            대규모내부거래관련(구)
         *              J004	            기업집단현황공시
         *              J005	            비상장회사중요사항공시
         *              J006	            기타공정위공시
         *
         * Response Result: ResDisclosureSearchResult
         */
        public ResDisclosureSearchResult REQ1_1_GET_DISCLOSURE_SEARCH(ReqDisclosureSearch rds, bool isXml = false)
        {
            debugBeginProtocol("REQ1_1_GET_DISCLOSURE_SEARCH");
            checkApiKey();

            ResDisclosureSearchResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request JSON format
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // https://opendart.fss.or.kr/api/list.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&bgn_de=20200117&end_de=20200117&corp_cls=Y&page_no=1&page_count=10
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(rds.corp_code)) reqParam += "&corp_code=" + rds.corp_code;
                if (!string.IsNullOrEmpty(rds.bgn_de)) reqParam += "&bgn_de=" + rds.bgn_de;
                if (!string.IsNullOrEmpty(rds.end_de)) reqParam += "&end_de=" + rds.end_de;
                if (!string.IsNullOrEmpty(rds.last_reprt_at)) reqParam += "&last_reprt_at=" + rds.last_reprt_at;
                if (!string.IsNullOrEmpty(rds.pblntf_ty)) reqParam += "&pblntf_ty=" + rds.pblntf_ty;
                if (!string.IsNullOrEmpty(rds.pblntf_detail_ty)) reqParam += "&pblntf_detail_ty=" + rds.pblntf_detail_ty;
                if (!string.IsNullOrEmpty(rds.corp_cls)) reqParam += "&corp_cls=" + rds.corp_cls;
                if (!string.IsNullOrEmpty(rds.sort)) reqParam += "&sort=" + rds.sort;
                if (!string.IsNullOrEmpty(rds.sort_mth)) reqParam += "&sort_mth=" + rds.sort_mth;
                if (!string.IsNullOrEmpty(rds.page_no)) reqParam += "&page_no=" + rds.page_no;
                if (!string.IsNullOrEmpty(rds.page_count)) reqParam += "&page_count=" + rds.page_count;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/list." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                    {"status":"000","message":"정상","page_no":1,"page_count":10,"total_count":223,"total_page":23,
                "list":[
                {"corp_code":"00120182","corp_name":"NH투자증권","stock_code":"005940","corp_cls":"Y","report_nm":"[첨부추가]일괄신고추가서류(파생결합증권-주가연계증권)","rcept_no":"20200117000559","flr_nm":"NH투자증권","rcept_dt":"20200117","rm":""},
                {"corp_code":"00120182","corp_name":"NH투자증권","stock_code":"005940","corp_cls":"Y","report_nm":"[첨부추가]일괄신고추가서류(파생결합증권-주가연계증권)","rcept_no":"20200117000486","flr_nm":"NH투자증권","rcept_dt":"20200117","rm":""},
                {"corp_code":"00120182","corp_name":"NH투자증권","stock_code":"005940","corp_cls":"Y","report_nm":"[첨부추가]일괄신고추가서류(파생결합증권-주가연계증권)","rcept_no":"20200117000375","flr_nm":"NH투자증권","rcept_dt":"20200117","rm":""},
                {"corp_code":"00120182","corp_name":"NH투자증권","stock_code":"005940","corp_cls":"Y","report_nm":"[첨부추가]일괄신고추가서류(파생결합증권-주가연계증권)","rcept_no":"20200117000341","flr_nm":"NH투자증권","rcept_dt":"20200117","rm":""},
                {"corp_code":"00120182","corp_name":"NH투자증권","stock_code":"005940","corp_cls":"Y","report_nm":"[첨부추가]일괄신고추가서류(파생결합증권-주가연계증권)","rcept_no":"20200117000083","flr_nm":"NH투자증권","rcept_dt":"20200117","rm":""},
                {"corp_code":"00120182","corp_name":"NH투자증권","stock_code":"005940","corp_cls":"Y","report_nm":"[첨부추가]일괄신고추가서류(파생결합증권-주가연계증권)","rcept_no":"20200117000030","flr_nm":"NH투자증권","rcept_dt":"20200117","rm":""},
                {"corp_code":"00878915","corp_name":"DGB금융지주","stock_code":"139130","corp_cls":"Y","report_nm":"소송등의판결ㆍ결정(자회사의 주요경영사항)","rcept_no":"20200117800593","flr_nm":"DGB금융지주","rcept_dt":"20200117","rm":"유"},
                {"corp_code":"00120571","corp_name":"롯데칠성음료","stock_code":"005300","corp_cls":"Y","report_nm":"타법인주식및출자증권취득결정","rcept_no":"20200117800584","flr_nm":"롯데칠성음료","rcept_dt":"20200117","rm":"유정"},
                {"corp_code":"00161709","corp_name":"퍼시스","stock_code":"016800","corp_cls":"Y","report_nm":"주식등의대량보유상황보고서(약식)","rcept_no":"20200117000661","flr_nm":"피델리티매니지먼트앤리서치컴퍼니엘엘씨","rcept_dt":"20200117","rm":""},
                {"corp_code":"00188089","corp_name":"한섬","stock_code":"020000","corp_cls":"Y","report_nm":"주식등의대량보유상황보고서(약식)","rcept_no":"20200117000657","flr_nm":"피델리티매니지먼트앤리서치컴퍼니엘엘씨","rcept_dt":"20200117","rm":""}
                ]}
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResDisclosureSearchResult));
                    result = (ResDisclosureSearchResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResDisclosureSearchResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 1. 공시정보
         * Api Name     : 1.2. 기업개황, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS001&apiId=2019002
         * Description  : DART에 등록되어있는 기업의 개황정보를 제공합니다.
         *                CORPCODE.zip -> CORPCODE.xml -> 구조체에 설정
         *              
         * Request URL  : https://opendart.fss.or.kr/api/company.json
         *                https://opendart.fss.or.kr/api/company.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	    STRING(40)      Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	     STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * Response Result: ResCompanyInfo
         */
        public ResCompanyInfo REQ1_2_GET_COMPANY_INFO(string corp_code, bool isXml = false)
        {
            debugBeginProtocol("REQ1_2_GET_COMPANY_INFO");
            checkApiKey();

            ResCompanyInfo result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request JSON format
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/company." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                    {"status":"000",
                     "message":"정상",
                     "corp_code":"00126380",
                     "corp_name":"삼성전자(주)",
                     "corp_name_eng":"SAMSUNG ELECTRONICS CO,.LTD",
                     "stock_name":"삼성전자",
                     "stock_code":"005930",
                     "ceo_nm":"김기남, 김현석, 고동진",
                     "corp_cls":"Y",
                     "jurir_no":"1301110006246",
                     "bizr_no":"1248100998",
                     "adres":"경기도 수원시 영통구  삼성로 129 (매탄동)",
                     "hm_url":"www.sec.co.kr",
                     "ir_url":"",
                     "phn_no":"031-200-1114",
                     "fax_no":"031-200-7538",
                     "induty_code":"264",
                     "est_dt":"19690113",
                     "acc_mt":"12"}
                 ----------------------------------------------------------------------*/
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResCompanyInfo));
                    result = (ResCompanyInfo)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResCompanyInfo>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 1. 공시정보
         * Api Name     : 1.3. 공시서류원본파일, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS001&apiId=2019003
         * Description  : 공시보고서 원본파일을 제공합니다.
         *                download directory: dummyDirectory/document/rcept_no
         *                ex) dummyDirectory/document/20190401004781/20190401004781.zip -> 20190401004781.xml
         *              
         * Request URL  : https://opendart.fss.or.kr/api/document.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	    STRING(40)	    Y	        발급받은 인증키(40자리)
         * rcept_no	    접수번호	     STRING(14)	     Y	        접수번호
         * 
         * Response Result: Zip FILE (binary)
         */
        public string REQ1_3_GET_DOCUMENT_FILE(string rcept_no)
        {
            debugBeginProtocol("REQ1_3_GET_DOCUMENT_FILE");
            checkApiKey();

            string result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request JSON format
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(rcept_no)) reqParam += "&rcept_no=" + rcept_no;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/document.xml" + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 ----------------------------------------------------------------------*/
                string zipDirectory = dummyDirectory + Path.DirectorySeparatorChar + "document" + Path.DirectorySeparatorChar + rcept_no;
                string zipFilePath = zipDirectory + Path.DirectorySeparatorChar + rcept_no + ".zip";
                if (Directory.Exists(zipDirectory))
                {
                    File.Delete(zipDirectory + Path.DirectorySeparatorChar + "*.*");
                    Directory.Delete(zipDirectory);
                }
                if (!Directory.Exists(zipDirectory))
                {
                    DirectoryInfo di = Directory.CreateDirectory(zipDirectory);
                }
                // 20190401004781.zip
                using (FileStream fs = new FileStream(zipFilePath, FileMode.Create, System.IO.FileAccess.Write))
                {
                    fs.Write(resData, 0, resData.Length);
                    fs.Close();
                    ZipFile.ExtractToDirectory(zipFilePath, zipDirectory);
                    // 20190401004781.xml
                    // 20190401004781_00760.xml
                    // 20190401004781_00761.xml
                }

                //using (StreamReader file = new StreamReader("DOCUMENT.xml"))
                //{
                //    XmlSerializer reader = new XmlSerializer(typeof(DummyCorps));
                //    //DummyCorps corps = (DummyCorps)reader.Deserialize(file);
                //    dummyCorps = (DummyCorps)reader.Deserialize(file);
                //    file.Close();
                //}

                result = zipDirectory;
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 1. 공시정보
         * Api Name     : 1.4. 고유번호, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS001&apiId=2019018
         * Description  : DART에 등록되어있는 공시대상회사의 고유번호,회사명,대표자명,종목코드, 최근변경일자를 파일로 제공합니다.
         *                CORPCODE.zip -> CORPCODE.xml -> 구조체에 설정
         *              
         * Request URL: https://opendart.fss.or.kr/api/corpCode.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key    API 인증키  STRING(40)   Y	        발급받은 인증키(40자리)
         *
         * Reponse Result:
         * 키	명칭	List 여부	출력설명
         *   status	에러 및 정보 코드		(※메시지 설명 참조)
         *   message	에러 및 정보 메시지		(※메시지 설명 참조)
         *   corp_code	고유번호	Y	공시대상회사의 고유번호(8자리)
         *   ※ ZIP File 안에 있는 XML파일 정보
         *   corp_name	정식명칭	Y	정식회사명칭
         *   ※ ZIP File 안에 있는 XML파일 정보
         *   stock_code	종목코드	Y	상장회사인 경우 주식의 종목코드(6자리)
         *   ※ ZIP File 안에 있는 XML파일 정보
         *   modify_date	최종변경일자	Y	기업개황정보 최종변경일자(YYYYMMDD)
         *   ※ ZIP File 안에 있는 XML파일 정보
         */
        public ResCorpCodeResult REQ1_4_GET_CORPCODE_INFO()
        {
            debugBeginProtocol("REQ1_4_GET_CORPCODE_INFO");
            checkApiKey();

            ResCorpCodeResult result = null;

            try
            {
                isLogin = false;
                // string id = Options.Instance.oi.loginInfo.ID;
                // string password = Utility.Instance.decryptString(Options.Instance.oi.loginInfo.PASSWORD, Utility.Instance.getMacAddress() + Options.Instance.oi.appInfo.KEY + Options.Instance.oi.loginInfo.ID);
                // password = Utility.Instance.getHashCode(password, Options.Instance.oi.loginInfo.PASSWORD_HASH_TYPE, Options.Instance.oi.loginInfo.PASSWORD_HASH_CAPSLOCK);

                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                 ----------------------------------------------------------------------*/
                // Serialize
                //Hero hero = new Hero();
                //reqJson = JsonSerializer.Serialize(Hero, JsonUtility.jsonSerializerOptionsCamelCase);
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                // if (!string.IsNullOrEmpty(rcept_no)) reqParam += "&rcept_no=" + rcept_no;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/corpCode.xml" + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Client-Key"] = Options.Instance.oi.appInfo.KEY;
                // request.Headers["X-Mac-Address"] = Utility.Instance.getMacAddress().ToUpper();
                // request.Headers["X-Instance-Id"] = Utility.Instance.instanceGuid;
                // request.Headers["X-Instance-Datetime"] = Utility.Instance.instanceDateTime;
                // request.Headers["X-User-Id"] = Options.Instance.oi.loginInfo.ID;
                // request.Headers["X-Protocol-Id"] = iip_Status.PROTOCOL_NAME;
                // request.Headers["X-Session-Id"] = iip_Status.LAST_SESSION_ID;
                // request.Headers["X-Result-Code"] = "0";
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData);

                //System.Diagnostics.Debug.WriteLine(Utility.Instance.HexDump(reqData));
                // BitConverter.ToString(reqData).Replace("-", string.Empty)
                // base64 : Converter.ToBase64String

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                //dataStream = response.GetResponseStream();
                //StreamReader reader = new StreamReader(dataStream);
                //responseJSON = reader.ReadToEnd();
                //reader.Close();
                //dataStream.Close();
                DebugResponse(response, resData);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 ----------------------------------------------------------------------*/
                File.Delete(dummyDirectory + Path.DirectorySeparatorChar + "CORPCODE.zip");
                File.Delete(dummyDirectory + Path.DirectorySeparatorChar + "CORPCODE.xml");
                using (FileStream fs = new FileStream(dummyDirectory + Path.DirectorySeparatorChar + "CORPCODE.zip", FileMode.Create, System.IO.FileAccess.Write))
                {
                    fs.Write(resData, 0, resData.Length);
                    fs.Close();
                    ZipFile.ExtractToDirectory(dummyDirectory + Path.DirectorySeparatorChar + "CORPCODE.zip", dummyDirectory);
                }

                using (StreamReader file = new StreamReader(dummyDirectory + Path.DirectorySeparatorChar + "CORPCODE.xml"))
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResCorpCodeResult));
                    //ResCorpCodeResult result = (ResCorpCodeResult)reader.Deserialize(file);
                    result = (ResCorpCodeResult)reader.Deserialize(file);
                    file.Close();
                }

                // corpCodeList.displayConsole();
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 2. 사업보고서 주요정보
         * Api Name     : 2.1. 증자(감자) 현황, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS002&apiId=2019004
         * Description  : 정기보고서(사업, 분기, 반기보고서) 내에 증자(감자) 현황을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/irdsSttus.json
         *                https://opendart.fss.or.kr/api/irdsSttus.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResIrdsSttusResult
         */
        public ResIrdsSttusResult REQ2_1_GET_IRDS_STTUS_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ2_1_GET_IRDS_STTUS_INFO");
            checkApiKey();

            ResIrdsSttusResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/irdsSttus.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/irdsSttus." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190820000266",
                      "corp_cls":"K",
                      "corp_code":"00293886",
                      "corp_name":"위닉스",
                      "isu_dcrs_de":"1986.09.01",
                      "isu_dcrs_stle":"현물출자",
                      "isu_dcrs_stock_knd":"보통주",
                      "isu_dcrs_qy":"40,000",
                      "isu_dcrs_mstvdv_fval_amount":"5,000",
                      "isu_dcrs_mstvdv_amount":"5,000"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResIrdsSttusResult));
                    result = (ResIrdsSttusResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResIrdsSttusResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 2. 사업보고서 주요정보
         * Api Name     : 2.2. 배당에 관한 사항, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS002&apiId=2019005
         * Description  : 정기보고서(사업, 분기, 반기보고서) 내에 배당에 관한 사항을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/alotMatter.json
         *                https://opendart.fss.or.kr/api/alotMatter.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResIrdsSttusResult
         */
        public ResAlotMatterResult REQ2_2_GET_ALOT_MATTER_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ2_2_GET_ALOT_MATTER_INFO");
            checkApiKey();

            ResAlotMatterResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/alotMatter.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/alotMatter." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190820000266",
                      "corp_cls":"K",
                      "corp_code":"00293886",
                      "corp_name":"위닉스",
                      "se":"주당액면가액(원)",
                      "thstrm":"500",
                      "frmtrm":"500",
                      "lwfr":"500"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResAlotMatterResult));
                    result = (ResAlotMatterResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResAlotMatterResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 2. 사업보고서 주요정보
         * Api Name     : 2.3. 자기주식 취득 및 처분 현황, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS002&apiId=2019006
         * Description  : 정기보고서(사업, 분기, 반기보고서) 내에 자기주식 취득 및 처분 현황을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/tesstkAcqsDspsSttus.json
         *                https://opendart.fss.or.kr/api/tesstkAcqsDspsSttus.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResTesstkAcqsDspsSttusResult
         */
        public ResTesstkAcqsDspsSttusResult REQ2_3_GET_TESSTK_ACQS_DSPS_STTUS_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ2_3_GET_TESSTK_ACQS_DSPS_STTUS_INFO");
            checkApiKey();

            ResTesstkAcqsDspsSttusResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/tesstkAcqsDspsSttus.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/tesstkAcqsDspsSttus." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190820000266",
                      "corp_cls":"K",
                      "corp_code":"00293886",
                      "corp_name":"위닉스",
                      "stock_knd":"보통주",
                      "acqs_mth1":"배당가능이익범위 이내 취득",
                      "acqs_mth2":"직접취득",
                      "acqs_mth3":"장내직접취득",
                      "bsis_qy":"0",
                      "change_qy_acqs":"0",
                      "change_qy_dsps":"0",
                      "change_qy_incnr":"0",
                      "trmend_qy":"0",
                      "rm":"-"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResTesstkAcqsDspsSttusResult));
                    result = (ResTesstkAcqsDspsSttusResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResTesstkAcqsDspsSttusResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 2. 사업보고서 주요정보
         * Api Name     : 2.4. 최대주주 현황, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS002&apiId=2019007
         * Description  : 정기보고서(사업, 분기, 반기보고서) 내에 최대주주 현황을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/hyslrSttus.json
         *                https://opendart.fss.or.kr/api/hyslrSttus.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResTesstkAcqsDspsSttusResult
         */
        public ResHyslrSttusResult REQ2_4_GET_HYSLR_STTUS_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ2_4_GET_HYSLR_STTUS_INFO");
            checkApiKey();

            ResHyslrSttusResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/hyslrSttus.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/hyslrSttus." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190820000266",
                      "corp_cls":"K",
                      "corp_code":"00293886",
                      "corp_name":"위닉스",
                      "stock_knd":"보통주",
                      "rm":"-",
                      "nm":"윤희종",
                      "relate":"본인",
                      "bsis_posesn_stock_co":
                      "5,455,971",
                      "bsis_posesn_stock_qota_rt":"33.35",
                      "trmend_posesn_stock_co":"5,455,971",
                      "trmend_posesn_stock_qota_rt":"30.53"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResHyslrSttusResult));
                    result = (ResHyslrSttusResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResHyslrSttusResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 2. 사업보고서 주요정보
         * Api Name     : 2.5. 최대주주 변동 현황, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS002&apiId=2019008
         * Description  : 정기보고서(사업, 분기, 반기보고서) 내에 최대주주 변동 현황을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/hyslrChgSttus.json
         *                https://opendart.fss.or.kr/api/hyslrChgSttus.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResTesstkAcqsDspsSttusResult
         */
        public ResHyslrChgSttusResult REQ2_5_GET_HYSLR_CHG_STTUS_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ2_5_GET_HYSLR_CHG_STTUS_INFO");
            checkApiKey();

            ResHyslrChgSttusResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/hyslrChgSttus.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/hyslrChgSttus." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190820000266",
                     "corp_cls":"K",
                     "corp_code":"00293886",
                     "corp_name":"위닉스",
                     "rm":"-",
                     "change_on":"-",
                     "mxmm_shrholdr_nm":"-",
                     "posesn_stock_co":"-",
                     "qota_rt":"-",
                     "change_cause":"-"}]}
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResHyslrChgSttusResult));
                    result = (ResHyslrChgSttusResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResHyslrChgSttusResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 2. 사업보고서 주요정보
         * Api Name     : 2.6. 소액주주현황, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS002&apiId=2019009
         * Description  : 정기보고서(사업, 분기, 반기보고서) 내에 소액주주현황을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/mrhlSttus.json
         *                https://opendart.fss.or.kr/api/mrhlSttus.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResTesstkAcqsDspsSttusResult
         */
        public ResMrhlSttusResult REQ2_6_GET_MRHL_STTUS_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ2_6_GET_MRHL_STTUS_INFO");
            checkApiKey();

            ResMrhlSttusResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/mrhlSttus.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/mrhlSttus." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190820000266",
                     "corp_cls":"K",
                     "corp_code":"00293886",
                     "corp_name":"위닉스",
                     "se":"소액주주",
                     "shrholdr_co":"12,093",
                     "shrholdr_tot_co":"-",
                     "shrholdr_rate":"99.94%",
                     "hold_stock_co":"6,617,302",
                     "stock_tot_co":"-",
                     "hold_stock_rate":"37.02%"}]}
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResMrhlSttusResult));
                    result = (ResMrhlSttusResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResMrhlSttusResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 2. 사업보고서 주요정보
         * Api Name     : 2.7. 임원현황, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS002&apiId=2019010
         * Description  : 정기보고서(사업, 분기, 반기보고서) 내에 임원현황을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/exctvSttus.json
         *                https://opendart.fss.or.kr/api/exctvSttus.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResExctvSttusResult
         */
        public ResExctvSttusResult REQ2_7_GET_EXCTV_STTUS_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ2_7_GET_EXCTV_STTUS_INFO");
            checkApiKey();

            ResExctvSttusResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/exctvSttus.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/exctvSttus." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190820000266",
                      "corp_cls":"K",
                      "corp_code":"00293886",
                      "corp_name":"위닉스",
                      "nm":"윤희종",
                      "sexdstn":"남",
                      "birth_ym":"1947년 10월",
                      "ofcps":"회장",
                      "rgist_exctv_at":"등기임원",
                      "fte_at":"상근",
                      "chrg_job":"경영총괄",
                      "main_career":"영남대 상경대학중퇴",
                      "mxmm_shrholdr_relate":"본인",
                      "hffc_pd":"32년",
                      "tenure_end_on":"2018년 12월 31일"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResExctvSttusResult));
                    result = (ResExctvSttusResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResExctvSttusResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 2. 사업보고서 주요정보
         * Api Name     : 2.8. 직원현황, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS002&apiId=2019011
         * Description  : 정기보고서(사업, 분기, 반기보고서) 내에 직원현황을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/empSttus.json
         *                https://opendart.fss.or.kr/api/empSttus.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResEmpSttusResult
         */
        public ResEmpSttusResult REQ2_8_GET_EMP_STTUS_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ2_8_GET_EMP_STTUS_INFO");
            checkApiKey();

            ResEmpSttusResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/empSttus.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/empSttus." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190820000266",
                      "corp_cls":"K",
                      "corp_code":"00293886",
                      "corp_name":"위닉스",
                      "rm":"-",
                      "sexdstn":"남",
                      "fo_bbm":"-",
                      "reform_bfe_emp_co_rgllbr":"-",
                      "reform_bfe_emp_co_cnttk":"-",
                      "reform_bfe_emp_co_etc":"-",
                      "rgllbr_co":"410",
                      "rgllbr_abacpt_labrr_co":"0",
                      "cnttk_co":"16",
                      "cnttk_abacpt_labrr_co":"0",
                      "sm":"426",
                      "avrg_cnwk_sdytrn":" 4년6 개월 ",
                      "fyer_salary_totamt":"21,757,719,000",
                      "jan_salary_am":"51,074,000"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResEmpSttusResult));
                    result = (ResEmpSttusResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResEmpSttusResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 2. 사업보고서 주요정보
         * Api Name     : 2.9. 이사ㆍ감사의 개인별 보수 현황, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS002&apiId=2019012
         * Description  : 정기보고서(사업, 분기, 반기보고서) 내에 이사ㆍ감사의 개인별 보수 현황을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/hmvAuditIndvdlBySttus.json
         *                https://opendart.fss.or.kr/api/hmvAuditIndvdlBySttus.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResHmvAuditIndvdlBySttusResult
         */
        public ResHmvAuditIndvdlBySttusResult REQ2_9_GET_HMV_AUDIT_INDVDL_BY_STTUS_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ2_9_GET_HMV_AUDIT_INDVDL_BY_STTUS_INFO");
            checkApiKey();

            ResHmvAuditIndvdlBySttusResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/hmvAuditIndvdlBySttus.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/hmvAuditIndvdlBySttus." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190820000266",
                      "corp_cls":"K",
                      "corp_code":"00293886",
                      "corp_name":"위닉스",
                      "nm":"윤철민",
                      "ofcps":"사장",
                      "mendng_totamt":"950,000,000",
                      "mendng_totamt_ct_incls_mendng":"-"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResHmvAuditIndvdlBySttusResult));
                    result = (ResHmvAuditIndvdlBySttusResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResHmvAuditIndvdlBySttusResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 2. 사업보고서 주요정보
         * Api Name     : 2.10. 이사ㆍ감사 전체의 보수현황, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS002&apiId=2019013
         * Description  : 정기보고서(사업, 분기, 반기보고서) 내에 이사ㆍ감사 전체의 보수현황을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/hmvAuditAllSttus.json
         *                https://opendart.fss.or.kr/api/hmvAuditAllSttus.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResHmvAuditAllSttusResult
         */
        public ResHmvAuditAllSttusResult REQ2_10_GET_HMV_AUDIT_ALL_STTUS_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ2_10_GET_HMV_AUDIT_ALL_STTUS_INFO");
            checkApiKey();

            ResHmvAuditAllSttusResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/hmvAuditAllSttus.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/hmvAuditAllSttus." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190820000266",
                      "corp_cls":"K",
                      "corp_code":"00293886",
                      "corp_name":"위닉스",
                      "rm":"-",
                      "nmpr":"5",
                      "jan_avrg_mendng_am":"402,800,000",
                      "mendng_totamt":"2,014,000,000"}]}
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResHmvAuditAllSttusResult));
                    result = (ResHmvAuditAllSttusResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResHmvAuditAllSttusResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 2. 사업보고서 주요정보
         * Api Name     : 2.11. 개인별 보수지급 금액(5억이상 상위5인), https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS002&apiId=2019014
         * Description  : 정기보고서(사업, 분기, 반기보고서) 내에 개인별 보수지급 금액(5억이상 상위5인)을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/indvdlByPay.json
         *                https://opendart.fss.or.kr/api/indvdlByPay.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResIndvdlByPayResult
         */
        public ResIndvdlByPayResult REQ2_11_GET_INDVDL_BY_PAY_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ2_11_GET_INDVDL_BY_PAY_INFO");
            checkApiKey();

            ResIndvdlByPayResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/indvdlByPay.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/indvdlByPay." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190820000266",
                      "corp_cls":"K",
                      "corp_code":"00293886",
                      "corp_name":"위닉스",
                      "nm":"윤희종",
                      "ofcps":"회장",
                      "mendng_totamt":"980,000,000",
                      "mendng_totamt_ct_incls_mendng":"-"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResIndvdlByPayResult));
                    result = (ResIndvdlByPayResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResIndvdlByPayResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 2. 사업보고서 주요정보
         * Api Name     : 2.12. 타법인 출자현황, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS002&apiId=2019015
         * Description  : 정기보고서(사업, 분기, 반기보고서) 내에 타법인 출자현황을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/otrCprInvstmntSttus.json
         *                https://opendart.fss.or.kr/api/otrCprInvstmntSttus.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResIndvdlByPayResult
         */
        public ResOtrCprInvstmntSttusResult REQ2_12_GET_OTR_CPR_INVSTMNT_STTUS_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ2_12_GET_OTR_CPR_INVSTMNT_STTUS_INFO");
            checkApiKey();

            ResOtrCprInvstmntSttusResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/otrCprInvstmntSttus.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/otrCprInvstmntSttus." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190820000266",
                      "corp_cls":"K",
                      "corp_code":"00293886",
                      "corp_name":"위닉스",
                      "inv_prm":"유원전자(소주)\n유한공사(비상장)",
                      "frst_acqs_de":"1997.04.18",
                      "invstmnt_purps":"계열회사",
                      "frst_acqs_amount":"4,832,000,000",
                      "bsis_blce_qy":"0",
                      "bsis_blce_qota_rt":"100",
                      "bsis_blce_acntbk_amount":"8,551,000,000",
                      "incrs_dcrs_acqs_dsps_qy":"0",
                      "incrs_dcrs_acqs_dsps_amount":"0",
                      "incrs_dcrs_evl_lstmn":"0",
                      "trmend_blce_qy":"0",
                      "trmend_blce_qota_rt":"100",
                      "trmend_blce_acntbk_amount":"8,551,000,000",
                      "recent_bsns_year_fnnr_sttus_tot_assets":"10,465,000,000",
                      "recent_bsns_year_fnnr_sttus_thstrm_ntpf":"-909,000,000"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResOtrCprInvstmntSttusResult));
                    result = (ResOtrCprInvstmntSttusResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResOtrCprInvstmntSttusResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 3. 상장기업 재무정보
         * Api Name     : 3.1. 단일회사 주요계정, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS003&apiId=2019016
         * Description  : 상장법인(금융업 제외)이 제출한 정기보고서 내에 XBRL재무제표의 주요계정과목(재무상태표, 손익계산서)을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/fnlttSinglAcnt.json
         *                https://opendart.fss.or.kr/api/fnlttSinglAcnt.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResFnlttSinglAcntResult
         */
        public ResFnlttSinglAcntResult REQ3_1_GET_FNLTT_SINGL_ACNT_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ3_1_GET_FNLTT_SINGL_ACNT_INFO");
            checkApiKey();

            ResFnlttSinglAcntResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/fnlttSinglAcnt.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/fnlttSinglAcnt." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190401004781",
                      "reprt_code":"11011",
                      "bsns_year":"2018",
                      "corp_code":"00126380",
                      "stock_code":"005930",
                      "fs_div":"CFS",
                      "fs_nm":"연결재무제표",
                      "sj_div":"BS",
                      "sj_nm":"재무상태표",
                      "account_nm":"유동자산",
                      "thstrm_nm":"제 50 기",
                      "thstrm_dt":"2018.12.31 현재",
                      "thstrm_amount":"174,697,424,000,000",
                      "frmtrm_nm":"제 49 기",
                      "frmtrm_dt":"2017.12.31 현재",
                      "frmtrm_amount":"146,982,464,000,000",
                      "bfefrmtrm_nm":"제 48 기",
                      "bfefrmtrm_dt":"2016.12.31 현재",
                      "bfefrmtrm_amount":"141,429,704,000,000",
                      "ord":"1"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResFnlttSinglAcntResult));
                    result = (ResFnlttSinglAcntResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResFnlttSinglAcntResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 3. 상장기업 재무정보
         * Api Name     : 3.2. 다중회사 주요계정, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS003&apiId=2019017
         * Description  : 상장법인(금융업 제외)이 제출한 정기보고서 내에 XBRL재무제표의 주요계정과목(재무상태표, 손익계산서)을 제공합니다. (상장법인 복수조회 가능)
         *              
         * Request URL  : https://opendart.fss.or.kr/api/fnlttMultiAcnt.json
         *                https://opendart.fss.or.kr/api/fnlttMultiAcnt.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * Response Result: ResFnlttMultiAcntResult
         */
        public ResFnlttMultiAcntResult REQ3_2_GET_FNLTT_MULTI_ACNT_INFO(string corp_code, string bsns_year, string reprt_code, bool isXml = false)
        {
            debugBeginProtocol("REQ3_2_GET_FNLTT_MULTI_ACNT_INFO");
            checkApiKey();

            ResFnlttMultiAcntResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/fnlttMultiAcnt.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00356370,00334624&bsns_year=2018&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/fnlttMultiAcnt." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190401004781",
                      "reprt_code":"11011",
                      "bsns_year":"2018",
                      "corp_code":"00126380",
                      "stock_code":"005930",
                      "fs_div":"CFS",
                      "fs_nm":"연결재무제표",
                      "sj_div":"BS",
                      "sj_nm":"재무상태표",
                      "account_nm":"유동자산",
                      "thstrm_nm":"제 50 기",
                      "thstrm_dt":"2018.12.31 현재",
                      "thstrm_amount":"174,697,424,000,000",
                      "frmtrm_nm":"제 49 기",
                      "frmtrm_dt":"2017.12.31 현재",
                      "frmtrm_amount":"146,982,464,000,000",
                      "bfefrmtrm_nm":"제 48 기",
                      "bfefrmtrm_dt":"2016.12.31 현재",
                      "bfefrmtrm_amount":"141,429,704,000,000",
                      "ord":"1"},
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResFnlttMultiAcntResult));
                    result = (ResFnlttMultiAcntResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResFnlttMultiAcntResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 3. 상장기업 재무정보
         * Api Name     : 3.3. 재무제표 원본파일(XBRL), https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS003&apiId=2019019
         * Description  : 상장법인이 제출한 정기보고서 내에 XBRL재무제표의 원본파일(XBRL)을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/fnlttXbrl.xml <- UTF-8	Zip FILE (binary)
         *
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키      STRING(40)	   Y	        발급받은 인증키(40자리)
         * rcept_no	    접수번호        STRING(8)	   Y	        접수번호 ※ 조회방법 : 공시검색API 호출 > 응답요청 값 rcept_no 추출
         * reprt_code	보고서 코드      STRING(5)	    Y	         1분기보고서 : 11013
         *                                                         반기보고서 : 11012
         *                                                         3분기보고서 : 11014
         *                                                         사업보고서 : 11011
         * Response Result: ResFnlttMultiAcntResult
         */
        public string REQ3_3_GET_FNLTT_XBRL_INFO(string rcept_no, string reprt_code)
        {
            debugBeginProtocol("REQ3_2_GET_FNLTT_XBRL_INFO");
            checkApiKey();

            string result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/fnlttXbrl.xml?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&rcept_no=20190401004781&reprt_code=11011
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(rcept_no)) reqParam += "&rcept_no=" + rcept_no;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/fnlttXbrl.xml" + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 ----------------------------------------------------------------------*/
                string zipDirectory = dummyDirectory + Path.DirectorySeparatorChar + rcept_no;
                string zipFilePath = zipDirectory + Path.DirectorySeparatorChar + rcept_no + ".zip";
                if (Directory.Exists(zipDirectory))
                {
                    File.Delete(zipDirectory + Path.DirectorySeparatorChar + "*.*");
                    Directory.Delete(zipDirectory);
                }
                if (!Directory.Exists(zipDirectory))
                {
                    DirectoryInfo di = Directory.CreateDirectory(zipDirectory);
                }
                // 20190401004781.zip
                using (FileStream fs = new FileStream(zipFilePath, FileMode.Create, System.IO.FileAccess.Write))
                {
                    fs.Write(resData, 0, resData.Length);
                    fs.Close();
                    ZipFile.ExtractToDirectory(zipFilePath, zipDirectory);
                    // 20190401004781.xml
                    // 20190401004781_00760.xml
                    // 20190401004781_00761.xml
                }

                //using (StreamReader file = new StreamReader("DOCUMENT.xml"))
                //{
                //    XmlSerializer reader = new XmlSerializer(typeof(DummyCorps));
                //    //DummyCorps corps = (DummyCorps)reader.Deserialize(file);
                //    dummyCorps = (DummyCorps)reader.Deserialize(file);
                //    file.Close();
                //}

                result = zipDirectory;
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 3. 상장기업 재무정보
         * Api Name     : 3.4. 단일회사 전체 재무제표, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS003&apiId=2019020
         * Description  : 상장법인(금융업 제외)이 제출한 정기보고서 내에 XBRL재무제표의 모든계정과목을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/fnlttSinglAcntAll.json
         *                https://opendart.fss.or.kr/api/fnlttSinglAcntAll.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	        공시대상회사의 고유번호(8자리)
         *                                                          ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bsns_year	사업연도	    STRING(4)	    Y	        사업연도(4자리)
         *                                                          ※ 2015년 이후 부터 정보제공
         * reprt_code	보고서 코드	    STRING(5)	    Y	        1분기보고서 : 11013
         *                                                      반기보고서 : 11012
         *                                                      3분기보고서 : 11014
         *                                                      사업보고서 : 11011
         * fs_div	    개별/연결구분	STRING(3)	    Y	        CFS:연결재무제표, OFS:재무제표
         *
         * Response Result: ResFnlttMultiAcntResult
         */
        public ResFnlttSinglAcntAllResult REQ3_4_GET_FNLTT_SINGL_ACNT_ALL_INFO(string corp_code, string bsns_year, string reprt_code, string fs_div, bool isXml = false)
        {
            debugBeginProtocol("REQ3_4_GET_FNLTT_SINGL_ACNT_ALL_INFO");
            checkApiKey();

            ResFnlttSinglAcntAllResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/fnlttSinglAcntAll.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380&bsns_year=2018&reprt_code=11011&fs_div=OFS
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;
                if (!string.IsNullOrEmpty(bsns_year)) reqParam += "&bsns_year=" + bsns_year;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&reprt_code=" + reprt_code;
                if (!string.IsNullOrEmpty(reprt_code)) reqParam += "&fs_div=" + fs_div;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/fnlttSinglAcntAll." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190401004781",
                      "reprt_code":"11011",
                      "bsns_year":"2018",
                      "corp_code":"00126380",
                      "sj_div":"BS",
                      "sj_nm":"재무상태표",
                      "account_id":"ifrs_CurrentAssets",
                      "account_nm":"유동자산",
                      "account_detail":"-",
                      "thstrm_nm":"제 50 기",
                      "thstrm_amount":"80039455000000",
                      "frmtrm_nm":"제 49 기",
                      "frmtrm_amount":"70155189000000",
                      "bfefrmtrm_nm":"제 48 기",
                      "bfefrmtrm_amount":"69981128000000",
                      "ord":"1"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResFnlttSinglAcntAllResult));
                    result = (ResFnlttSinglAcntAllResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResFnlttSinglAcntAllResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        // 재무제표구분
        public enum SJ_DIV {
            BS1,BS2,BS3,BS4
            ,IS1,IS2,IS3,IS4
            ,CIS1,CIS2,CIS3,CIS4
            ,DCIS1,DCIS2,DCIS3,DCIS4,DCIS5,DCIS6,DCIS7,DCIS8
            ,CF1,CF2,CF3,CF4
            ,SCE1,SCE2
        }

        /******************************************************************************************************************************************************
         * Api Category : 3. 상장기업 재무정보
         * Api Name     : 3.5. XBRL택사노미재무제표양식, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS003&apiId=2020001
         * Description  : 금융감독원 회계포탈에서 제공하는 IFRS 기반 XBRL 재무제표 공시용 표준계정과목체계(계정과목) 을 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/xbrlTaxonomy.json
         *                https://opendart.fss.or.kr/api/xbrlTaxonomy.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * sj_div	    재무제표구분	 STRING(5)	    Y	         (※재무제표구분 참조)
         * 
         * 재무제표구분     재무제표명칭        개별/연결       표시방법        세전세후
         *   BS1	    재무상태표	           연결	       유동/비유동법	
         *   BS2	    재무상태표	           개별	       유동/비유동법	
         *   BS3	    재무상태표	           연결	       유동성배열법	
         *   BS4	    재무상태표	           개별	       유동성배열법	
         *   IS1	    별개의 손익계산서	   연결	        기능별분류	
         *   IS2	    별개의 손익계산서	   개별	        기능별분류	
         *   IS3	    별개의 손익계산서	   연결	        성격별분류	
         *   IS4	    별개의 손익계산서	   개별	        성격별분류	
         *   CIS1	    포괄손익계산서	       연결	        세후	
         *   CIS2	    포괄손익계산서	       개별	        세후	
         *   CIS3	    포괄손익계산서	       연결	        세전	
         *   CIS4	    포괄손익계산서	       개별	        세전	
         *   DCIS1	    단일 포괄손익계산서	    연결	    기능별분류	        세후포괄손익
         *   DCIS2	    단일 포괄손익계산서	    개별	    기능별분류	        세후포괄손익
         *   DCIS3	    단일 포괄손익계산서	    연결	    기능별분류	        세전
         *   DCIS4	    단일 포괄손익계산서	    개별	    기능별분류	        세전
         *   DCIS5	    단일 포괄손익계산서	    연결	    성격별분류	        세후포괄손익
         *   DCIS6	    단일 포괄손익계산서	    개별	    성격별분류	        세후포괄손익
         *   DCIS7	    단일 포괄손익계산서	    연결	    성격별분류	        세전
         *   DCIS8	    단일 포괄손익계산서	    개별	    성격별분류	        세전
         *   CF1	    현금흐름표	          연결	        직접법	
         *   CF2	    현금흐름표	          개별	        직접법	
         *   CF3	    현금흐름표	          연결	        간접법	
         *   CF4	    현금흐름표	          개별	        간접법	
         *   SCE1	    자본변동표	          연결		
         *   SCE2	    자본변동표	          개별	
         *
         * Response Result: ResFnlttMultiAcntResult
         */
        public ResXbrlTaxonomyResult REQ3_5_GET_XBRL_TAXONOMY_INFO(SJ_DIV sj_div, bool isXml = false)
        {
            debugBeginProtocol("REQ3_5_GET_XBRL_TAXONOMY_INFO");
            checkApiKey();

            ResXbrlTaxonomyResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/xbrlTaxonomy.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&sj_div=BS1
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                // if (!string.IsNullOrEmpty(sj_div)) reqParam += "&sj_div=" + sj_div;
                reqParam += "&sj_div=" + sj_div.ToString();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/xbrlTaxonomy." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"sj_div":"BS1",
                      "bsns_de":"20180701",
                      "account_id":"ifrs_StatementOfFinancialPositionAbstract",
                      "account_nm":"StatementOfFinancialPositionAbstract",
                      "label_kor":"재무상태표 [abstract]",
                      "label_eng":"Statement of financial position [abstract]",
                      "ifrs_ref":" "},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResXbrlTaxonomyResult));
                    result = (ResXbrlTaxonomyResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResXbrlTaxonomyResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 4. 지분공시 종합정보
         * Api Name     : 4.1. 대량보유 상황보고, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS004&apiId=2019021
         * Description  : 주식등의 대량보유상황보고서 내에 대량보유 상황보고 정보를 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/majorstock.json
         *                https://opendart.fss.or.kr/api/majorstock.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	         공시대상회사의 고유번호(8자리)
         *                                                         ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * 
         * Response Result: ResMajorstockResult
         */
        public ResMajorstockResult REQ4_1_GET_MAJORSTOCK_INFO(string corp_code, bool isXml = false)
        {
            debugBeginProtocol("REQ4_1_GET_MAJORSTOCK_INFO");
            checkApiKey();

            ResMajorstockResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/majorstock.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/majorstock." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20200207000636",
                      "rcept_dt":"2020-02-07",
                      "corp_code":"00126380",
                      "corp_name":"삼성전자",
                      "report_tp":"약식",
                      "repror":"국민연금공단",
                      "stkqy":"638,070,003",
                      "stkqy_irds":"37,907,275",
                      "stkrt":"10.69",
                      "stkrt_irds":"0.64",
                      "ctr_stkqy":"-",
                      "ctr_stkrt":"-",
                      "report_resn":"단순투자목적에서 일반투자목적으로 보유목적 변경"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResMajorstockResult));
                    result = (ResMajorstockResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResMajorstockResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        /******************************************************************************************************************************************************
         * Api Category : 4. 지분공시 종합정보
         * Api Name     : 4.2. 임원ㆍ주요주주 소유보고, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS004&apiId=2019022
         * Description  : 임원ㆍ주요주주특정증권등 소유상황보고서 내에 임원ㆍ주요주주 소유보고 정보를 제공합니다.
         *              
         * Request URL  : https://opendart.fss.or.kr/api/elestock.json
         *                https://opendart.fss.or.kr/api/elestock.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	   STRING(40)	   Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	    Y	         공시대상회사의 고유번호(8자리)
         *                                                         ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * 
         * Response Result: ResElestockResult
         */
        public ResElestockResult REQ4_2_GET_ELESTOCK_INFO(string corp_code, bool isXml = false)
        {
            debugBeginProtocol("REQ4_2_GET_ELESTOCK_INFO");
            checkApiKey();

            ResElestockResult result = null;

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request param
                https://opendart.fss.or.kr/api/elestock.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&corp_code=00126380
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = "?crtfc_key=" + apiKey;
                if (!string.IsNullOrEmpty(corp_code)) reqParam += "&corp_code=" + corp_code;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri + "/elestock." + (isXml ? "xml" : "json") + reqParam);
                request.ProtocolVersion = HttpVersion.Version11;
                if (useProxy)
                {
                    request.Proxy = new WebProxy(proxyIp, proxyPort);
                }
                //request.Credentials = CredentialCache.DefaultCredentials;
                //request.CookieContainer = new CookieContainer();
                //if (cookiecollection != null) request.CookieContainer.Add(cookiecollection);
                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.Timeout = timeOut * 1000;
                request.UserAgent = "Stock Valuator Client";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = reqData.Length;
                // request.Headers["X-Result-Message"] = "OK";
                if (reqData.Length > 0 && request.Method != "GET")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(reqData, 0, reqData.Length);
                    dataStream.Close();
                }
                DebugRequest(request, reqData, true);

                // HTTP Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] resData = ms.ToArray();
                DebugResponse(response, resData, true);
                ms.Close();
                response.Close();

                /*------------------------------------------------<<Response JSON format
                 {"status":"000","message":"정상","list":[
                     {"rcept_no":"20190516000064",
                      "rcept_dt":"2019-05-16",
                      "corp_code":"00126380",
                      "corp_name":"삼성전자",
                      "repror":"강봉구",
                      "isu_exctv_rgist_at":"비등기임원",
                      "isu_exctv_ofcps":"부사장",
                      "isu_main_shrholdr":"-",
                      "sp_stock_lmp_cnt":"2,000",
                      "sp_stock_lmp_irds_cnt":"2,000",
                      "sp_stock_lmp_rate":"0.00",
                      "sp_stock_lmp_irds_rate":"0.00"},...
                 ----------------------------------------------------------------------*/

                //// Descrialize
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(ResElestockResult));
                    result = (ResElestockResult)reader.Deserialize(new MemoryStream(resData));
                    // result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    result = JsonSerializer.Deserialize<ResElestockResult>(resJson);
                    // result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                throw;
            }
            finally
            {
                debugEndProtocol();
            }

            return result;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        // PROTOCOL - END
        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        //=====================================================================================================================================================
        //=====================================================================================================================================================
    }
}
