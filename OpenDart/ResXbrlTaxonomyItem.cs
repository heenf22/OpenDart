using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenDart.Models
{
    public class ResXbrlTaxonomyItem
    {
        [XmlElement("sj_div")]
        public string sj_div { get; set; }      //	재무제표구분	Y	재무제표구분
        [XmlElement("account_id")]
        public string account_id { get; set; }  //	계정ID	Y	계정 고유명칭
        [XmlElement("account_nm")]
        public string account_nm { get; set; }  //	계정명	Y	계정명
        [XmlElement("bsns_de")]
        public string bsns_de { get; set; }     //	기준일	Y	적용 기준일
        [XmlElement("label_kor")]
        public string label_kor { get; set; }   //	한글 출력명	Y	한글 출력명
        [XmlElement("label_eng")]
        public string label_eng { get; set; }   //	영문 출력명	Y	영문 출력명
        [XmlElement("data_tp")]
        public string data_tp { get; set; }     //	데이터 유형	Y	※ 데이타 유형설명
                                                // - text block : 제목
                                                // - Text : Text
                                                // - yyyy-mm-dd : Date
                                                // - X : Monetary Value
                                                // - (X): Monetary Value(Negative)
                                                // - X.XX : Decimalized Value
                                                // - Shares : Number of shares (주식 수)
                                                // - For each : 공시된 항목이 전후로 반복적으로 공시될 경우 사용
                                                // - 공란 : 입력 필요 없음
        [XmlElement("ifrs_ref")]
        public string ifrs_ref { get; set; }    //	IFRS Reference	Y	IFRS Reference
                                                // ※ 출력예시
                                                // K-IFRS 1001 문단 54？(8), K-IFRS 1001 문단 78？(2)

        public ResXbrlTaxonomyItem()
        {
            sj_div = "";
            account_id = "";
            account_nm = "";
            bsns_de = "";
            label_kor = "";
            label_eng = "";
            data_tp = "";
            ifrs_ref = "";
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ResXbrlTaxonomyItem Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("sj_div: {0}", sj_div);
            Console.WriteLine("account_id: {0}", account_id);
            Console.WriteLine("account_nm: {0}", account_nm);
            Console.WriteLine("bsns_de: {0}", bsns_de);
            Console.WriteLine("label_kor: {0}", label_kor);
            Console.WriteLine("label_eng: {0}", label_eng);
            Console.WriteLine("data_tp: {0}", data_tp);
            Console.WriteLine("ifrs_ref: {0}", ifrs_ref);
            Console.WriteLine("==================================================");
        }
    }
}