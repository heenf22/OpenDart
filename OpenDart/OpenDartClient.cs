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

    public sealed class OpenDartClient
    {
        private static volatile OpenDartClient instance;
        private static object syncRoot = new Object();

        private OpenDartClient()
        {
            dummyDirectory = @"C:\Users\heenf\Desktop\Project\dummy";
            dummyCorps = new DummyCorps();
            requestApiKeyCount = 0;

            // X.509 SSL Define (private OCP) SSL 통신을 위해 CertificatePolicy property 등록
            //System.Net.ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            // or
            ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |      // TLS 1.0
            //                                       SecurityProtocolType.Tls11 |    // TLS 1.1
            //                                       SecurityProtocolType.Tls12 |    // TLS 1.2   
            //                                       SecurityProtocolType.Tls13;     // TLS 1.3
        }

        // X.509 SSL Define (private OCP)
        //public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //{
        //    return true;
        //}

        public static OpenDartClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new OpenDartClient();
                        }
                    }
                }

                return instance;
            }
        }

        public string dummyDirectory { get; set; }
        public int timeOut { get; set; } = 300;
        public bool useProxy { get; set; } = false;
        public string proxyIp { get; set; } = "127.0.0.1";
        public int proxyPort { get; set; } = 8080;
        public bool isLogin { get; set; } = false;
        public REQ_RESULT_STATUS reqResultStatus;

        // Open DART 쿼리용 키로 회원가입 후 발급받아 사용, 하루 이용횟수에 제한이 있음
        public string apiKey { get; set; }
        public string apiUri { get; set; } = "https://opendart.fss.or.kr/api";
        public int requestApiKeyCount { get; }

        private CookieCollection cookiecollection;

        public DummyCorps dummyCorps
        {
            get; set;
        }

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

        private void DebugBeginProtocol(string protocolName)
        {
            reqResultStatus.EXCEPTION = "";
            reqResultStatus.BEGIN_TICKCOUNT = Environment.TickCount;
            reqResultStatus.PROTOCOL_NAME = protocolName;
            Console.WriteLine("");
            Console.WriteLine("---- Begin {0}: {1}", reqResultStatus.PROTOCOL_NAME, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        }

        private void DebugEndProtocol()
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

            parseResult(response);
        }

        private bool parseResult(HttpWebResponse response)
        {
            try
            {
                reqResultStatus.CODE = response.GetResponseHeader("X-Result-Code");
                reqResultStatus.MESSAGE = response.GetResponseHeader("X-Result-Message");
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("!! NullReferenceException:StackTrace : " + e.StackTrace);
                Console.WriteLine("!! NullReferenceException:Message : " + e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("!! Exception:StackTrace : " + e.StackTrace);
                Console.WriteLine("!! Exception:Message : " + e.Message);
                return false;
            }

            return true;
        }

        private void displayWebException(WebException e)
        {
            Console.WriteLine("*******************************************************************************");
            Console.WriteLine("!!! Web EXCEPTION: " + e.Message);
            reqResultStatus.EXCEPTION = e.Message;
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

        //=====================================================================================================================================================
        //=====================================================================================================================================================
        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        // PROTOCOL - START
        //-----------------------------------------------------------------------------------------------------------------------------------------------------


        /******************************************************************************************************************************************************
         * Api Category : 공시정보
         * Api Name     : 1. 공시검색, https://opendart.fss.or.kr/guide/main.do?apiGrpCd=DS001
         * Description  : 공시 유형별, 회사별, 날짜별 등 여러가지 조건으로 공시보고서 검색기능을 제공합니다.
         *              
         * Request URL: https://opendart.fss.or.kr/api/list.json
         *              https://opendart.fss.or.kr/api/list.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	STRING(40)	Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	N	        공시대상회사의 고유번호(8자리)
         *                                                  ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * bgn_de	    시작일	    STRING(8)	N	        검색시작 접수일자(YYYYMMDD) : 없으면 종료일(end_de)
         *                                                  고유번호(corp_code)가 없는 경우 검색기간은 3개월로 제한
         * end_de	    종료일	    STRING(8)	N	        검색종료 접수일자(YYYYMMDD) : 없으면 당일
         * last_reprt_at최종보고서 검색여부	STRING(1)	N	최종보고서만 검색여부(Y or N) 기본값 : N
         *                                                  (정정이 있는 경우 최종정정만 검색)
         * pblntf_ty	공시유형	    STRING(1)	N	        A : 정기공시
         *                                                  B : 주요사항보고
         *                                                  C : 발행공시
         *                                                  D : 지분공시
         *                                                  E : 기타공시
         *                                                  F : 외부감사관련
         *                                                  G : 펀드공시
         *                                                  H : 자산유동화
         *                                                  I : 거래소공시
         *                                                  j : 공정위공시
         * pblntf_detail_ty	공시상세유형	STRING(4)	N	    (※ 상세 유형 참조 : pblntf_detail_ty)
         * corp_cls	    법인구분	    STRING(1)	N	        법인구분 : Y(유가), K(코스닥), N(코넥스), E(기타)
         *                                                  ※ 없으면 전체조회, 복수조건 불가
         * sort	        정렬	        STRING(4)	N	        접수일자: date
         *                                                  회사명 : crp
         *                                                  보고서명 : rpt
         *                                                  기본값 : date
         * sort_mth	    정렬방법	    STRING(4)	N	        오름차순(asc), 내림차순(desc) 기본값 : desc
         * page_no	    페이지 번호	STRING(5)	N	        페이지 번호(1~n) 기본값 : 1
         * page_count	페이지 별 건수	STRING(3)	N	    페이지당 건수(1~100) 기본값 : 10, 최대값 : 100
         * 
         * Response Result:
         * 
         * Response Status:
         *  - 000 :정상
         *  - 010 :등록되지 않은 키입니다.
         *  - 011 :사용할 수 없는 키입니다. 오픈API에 등록되었으나, 일시적으로 사용 중지된 키를 통하여 검색하는 경우 발생합니다.
         *  - 020 :요청 제한을 초과하였습니다.
         *         일반적으로는 10,000건 이상의 요청에 대하여 이 에러 메시지가 발생되나, 요청 제한이 다르게 설정된 경우에는 이에 준하여 발생됩니다.
         *  - 100 :필드의 부적절한 값입니다.필드 설명에 없는 값을 사용한 경우에 발생하는 메시지입니다.
         *  - 800 :원활한 공시서비스를 위하여 오픈API 서비스가 중지 중입니다.
         *  - 900 :정의되지 않은 오류가 발생하였습니다.
         *  string status = response.GetResponseHeader("status");
         *  string message = response.GetResponseHeader("message");
         */
        public bool REQ1_1_GET_DISCLOSURE_SEARCH(ReqDisclosureSearch rds, bool isXml = false)
        {
            DebugBeginProtocol("REQ1_1_GET_DISCLOSURE_SEARCH");

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request JSON format
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request(ex: https://opendart.fss.or.kr/api/list.json?crtfc_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&bgn_de=20200117&end_de=20200117&corp_cls=Y&page_no=1&page_count=10)
                string reqParam = string.Empty;
                if (string.IsNullOrEmpty(apiKey))
                {
                    Console.WriteLine("Could not find the api key. Please set the api key.");
                    return false;
                }
                reqParam += "?crtfc_key=" + apiKey;
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
                    ResDisclosureSearchResult result = (ResDisclosureSearchResult)reader.Deserialize(new MemoryStream(resData));
                    result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    //ResDisclosureSearchResult result = new ResDisclosureSearchResult();
                    ResDisclosureSearchResult result = JsonSerializer.Deserialize<ResDisclosureSearchResult>(resJson);
                    result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                return false;
            }
            finally
            {
                DebugEndProtocol();
            }

            return true;
        }

        /******************************************************************************************************************************************************
         * Api Category : 공시정보
         * Api Name     : 2. 기업개황, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS001&apiId=2019002
         * Description  : DART에 등록되어있는 기업의 개황정보를 제공합니다.
         *                CORPCODE.zip -> CORPCODE.xml -> 구조체에 설정
         *              
         * Request URL  : https://opendart.fss.or.kr/api/company.json
         *                https://opendart.fss.or.kr/api/company.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	STRING(40)  Y	        발급받은 인증키(40자리)
         * corp_code	고유번호	    STRING(8)	Y	        공시대상회사의 고유번호(8자리)
         *                                                  ※ 개발가이드 > 공시정보 > 고유번호 API조회 가능
         * Response Result: CompanyInfo
         * 
         * Response Status:
         *  - 000 :정상
         *  - 010 :등록되지 않은 키입니다.
         *  - 011 :사용할 수 없는 키입니다. 오픈API에 등록되었으나, 일시적으로 사용 중지된 키를 통하여 검색하는 경우 발생합니다.
         *  - 020 :요청 제한을 초과하였습니다.
         *         일반적으로는 10,000건 이상의 요청에 대하여 이 에러 메시지가 발생되나, 요청 제한이 다르게 설정된 경우에는 이에 준하여 발생됩니다.
         *  - 100 :필드의 부적절한 값입니다.필드 설명에 없는 값을 사용한 경우에 발생하는 메시지입니다.
         *  - 800 :원활한 공시서비스를 위하여 오픈API 서비스가 중지 중입니다.
         *  - 900 :정의되지 않은 오류가 발생하였습니다.
         *  string status = response.GetResponseHeader("status");
         *  string message = response.GetResponseHeader("message");
         */
        public bool REQ1_2_GET_COMPANY_INFO(string corp_code, bool isXml = false)
        {
            DebugBeginProtocol("REQ1_2_GET_COMPANY_INFO");

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request JSON format
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = string.Empty;
                if (string.IsNullOrEmpty(apiKey))
                {
                    Console.WriteLine("Could not find the api key. Please set the api key.");
                    return false;
                }
                reqParam += "?crtfc_key=" + apiKey;
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
                    {"status":"000","message":"정상","corp_code":"00126380","corp_name":"삼성전자(주)","corp_name_eng":"SAMSUNG ELECTRONICS CO,.LTD","stock_name":"삼성전자","stock_code":"005930","ceo_nm":"김기남, 김현석, 고동진","corp_cls":"Y","jurir_no":"1301110006246","bizr_no":"1248100998","adres":"경기도 수원시 영통구  삼성로 129 (매탄동)","hm_url":"www.sec.co.kr","ir_url":"","phn_no":"031-200-1114","fax_no":"031-200-7538","induty_code":"264","est_dt":"19690113","acc_mt":"12"}
                 ----------------------------------------------------------------------*/
                if (isXml)
                {
                    XmlSerializer reader = new XmlSerializer(typeof(CompanyInfo));
                    CompanyInfo result = (CompanyInfo)reader.Deserialize(new MemoryStream(resData));
                    result.displayConsole();
                }
                else
                {
                    resJson = Encoding.UTF8.GetString(resData);
                    // Descrialize
                    //CompanyInfo ci = new CompanyInfo();
                    CompanyInfo result = JsonSerializer.Deserialize<CompanyInfo>(resJson);
                    result.displayConsole();
                }
            }
            catch (WebException e)
            {
                displayWebException(e);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                return false;
            }
            finally
            {
                DebugEndProtocol();
            }

            return true;
        }

        /******************************************************************************************************************************************************
         * Api Category : 공시정보
         * Api Name     : 3. 공시서류원본파일, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS001&apiId=2019003
         * Description  : 공시보고서 원본파일을 제공합니다.
         *              20190401004781.zip -> 20190401004781.xml
         *              
         * Request URL  : https://opendart.fss.or.kr/api/document.xml
         * Request Parameter:
         * 키	        명칭	        타입	        필수여부	    값설명
         * crtfc_key	API 인증키	STRING(40)	Y	        발급받은 인증키(40자리)
         * rcept_no	    접수번호	    STRING(14)	Y	        접수번호
         * 
         * Response Result: Zip FILE (binary)
         * Response Status:
         *  - 000 :정상
         *  - 010 :등록되지 않은 키입니다.
         *  - 011 :사용할 수 없는 키입니다. 오픈API에 등록되었으나, 일시적으로 사용 중지된 키를 통하여 검색하는 경우 발생합니다.
         *  - 020 :요청 제한을 초과하였습니다.
         *         일반적으로는 10,000건 이상의 요청에 대하여 이 에러 메시지가 발생되나, 요청 제한이 다르게 설정된 경우에는 이에 준하여 발생됩니다.
         *  - 100 :필드의 부적절한 값입니다.필드 설명에 없는 값을 사용한 경우에 발생하는 메시지입니다.
         *  - 800 :원활한 공시서비스를 위하여 오픈API 서비스가 중지 중입니다.
         *  - 900 :정의되지 않은 오류가 발생하였습니다.
         *  string status = response.GetResponseHeader("status");
         *  string message = response.GetResponseHeader("message");
         */
        public bool REQ1_3_GET_DOCUMENT(string rcept_no)
        {
            DebugBeginProtocol("REQ1_3_GET_DOCUMENT");

            try
            {
                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request JSON format
                 ----------------------------------------------------------------------*/
                // Serialize
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = string.Empty;
                if (string.IsNullOrEmpty(apiKey))
                {
                    Console.WriteLine("Could not find the api key. Please set the api key.");
                    return false;
                }
                reqParam += "?crtfc_key=" + apiKey;
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
            }
            catch (WebException e)
            {
                displayWebException(e);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                return false;
            }
            finally
            {
                DebugEndProtocol();
            }

            return true;
        }

        /******************************************************************************************************************************************************
         * Api Category : 공시정보
         * Api Name     : 4. 고유번호, https://opendart.fss.or.kr/guide/detail.do?apiGrpCd=DS001&apiId=2019018
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
         * Response Status:
         *  - 000 :정상
         *  - 010 :등록되지 않은 키입니다.
         *  - 011 :사용할 수 없는 키입니다. 오픈API에 등록되었으나, 일시적으로 사용 중지된 키를 통하여 검색하는 경우 발생합니다.
         *  - 020 :요청 제한을 초과하였습니다.
         *         일반적으로는 10,000건 이상의 요청에 대하여 이 에러 메시지가 발생되나, 요청 제한이 다르게 설정된 경우에는 이에 준하여 발생됩니다.
         *  - 100 :필드의 부적절한 값입니다.필드 설명에 없는 값을 사용한 경우에 발생하는 메시지입니다.
         *  - 800 :원활한 공시서비스를 위하여 오픈API 서비스가 중지 중입니다.
         *  - 900 :정의되지 않은 오류가 발생하였습니다.
         *  string status = response.GetResponseHeader("status");
         *  string message = response.GetResponseHeader("message");
         */
        public bool REQ1_4_GET_CORPCODE()
        {
            DebugBeginProtocol("REQ1_4_GET_CORPCODE");

            try
            {
                isLogin = false;
                // string id = Options.Instance.oi.loginInfo.ID;
                // string password = Utility.Instance.decryptString(Options.Instance.oi.loginInfo.PASSWORD, Utility.Instance.getMacAddress() + Options.Instance.oi.appInfo.KEY + Options.Instance.oi.loginInfo.ID);
                // password = Utility.Instance.getHashCode(password, Options.Instance.oi.loginInfo.PASSWORD_HASH_TYPE, Options.Instance.oi.loginInfo.PASSWORD_HASH_CAPSLOCK);

                string reqJson = string.Empty;
                string resJson = string.Empty;

                /*------------------------------------------------->>Request JSON format
                    {
                        "In"{"LOGIN_INFO":{"ID":"lgh","Password":"eai"}}
                    }
                 ----------------------------------------------------------------------*/
                // Serialize
                //Hero hero = new Hero();
                //reqJson = JsonSerializer.Serialize(Hero, JsonUtility.jsonSerializerOptionsCamelCase);
                byte[] reqData = Encoding.UTF8.GetBytes(reqJson);

                // HTTP Request
                string reqParam = string.Empty;
                if (string.IsNullOrEmpty(apiKey))
                {
                    Console.WriteLine("Could not find the api key. Please set the api key.");
                    return false;
                }
                reqParam += "?crtfc_key=" + apiKey;
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
                    {"id":"test1","employeeId":"20210209-114403679","groupIds":[],"names":["test1"],"phoneNumbers":[],"emails":[],"passwordHashType":3,"password":"c3c7026420480e268ea803ec0c298122948dee6639283641d4d7677b59c3b4f35c0103257ab65bbc6d0d44859ef958cb423a6371b7ec553de5e2bb63cecae81e","nick":"","level":0,"isLogined":false,"chatStatus":0,"face":"","informations":[],"attribute":{"key":"bcf82789-f5fc-4117-a8d9-a4142846d292","enabled":true,"locked":false,"modifiers":2,"expireDateTime":"20210509114403679","comments":null,"regHeroId":"phantom","regDateTime":"20210209114403679","regTimeZoneId":"Korea Standard Time","modHeroId":"phantom","modDateTime":"20210209114403679","modTimeZoneId":"Korea Standard Time","testDt":"2021-02-09T11:44:03.6798662Z"}},{"id":"test2","employeeId":"20210209-114435078","groupIds":[],"names":["test2"],"phoneNumbers":[],"emails":[],"passwordHashType":3,"password":"c3c7026420480e268ea803ec0c298122948dee6639283641d4d7677b59c3b4f35c0103257ab65bbc6d0d44859ef958cb423a6371b7ec553de5e2bb63cecae81e","nick":"","level":0,"isLogined":false,"chatStatus":0,"face":"","informations":[],"attribute":{"key":"85d8db9b-686a-437b-a50d-ae53caa6e217","enabled":true,"locked":false,"modifiers":2,"expireDateTime":"20210509114435079","comments":null,"regHeroId":"phantom","regDateTime":"20210209114435079","regTimeZoneId":"Korea Standard Time","modHeroId":"phantom","modDateTime":"20210209114435079","modTimeZoneId":"Korea Standard Time","testDt":"2021-02-09T11:44:35.0790087Z"}}
                 ----------------------------------------------------------------------*/
                //resJson = Encoding.UTF8.GetString(resData);
                // #if DEBUG
                // #else
                // Console.WriteLine("{0}", resJson);
                // #endif
                // Descrialize
                //Hero[] heroes = JsonSerializer.Deserialize<Hero[]>(resJson);
                //Console.WriteLine("---------------");
                //foreach (Hero e in heroes)
                //{
                //    Console.WriteLine("Hero ID: {0}", e.id);
                //}
                //using (XmlTextReader xtr = new XmlTextReader("CORPCODE.xml"))
                //{
                //    while (xtr.Read())
                //    {

                //    }

                //    xtr.Close();
                //}

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
                    XmlSerializer reader = new XmlSerializer(typeof(DummyCorps));
                    //DummyCorps corps = (DummyCorps)reader.Deserialize(file);
                    dummyCorps = (DummyCorps)reader.Deserialize(file);
                    file.Close();
                }

                //foreach (DummyCorp corp in dummyCorps.Dummys)
                //{
                //    corp.displayConsole();
                //}

                Console.WriteLine("---------------");
            }
            catch (WebException e)
            {
                displayWebException(e);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("!!! EXCEPTION: " + e.Message);
                Console.WriteLine("*******************************************************************************");
                return false;
            }
            finally
            {
                DebugEndProtocol();
            }

            return true;
        }
    }
}
