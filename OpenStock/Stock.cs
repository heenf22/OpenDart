using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenStock
{
    public class Stock
    {
        public string code { get; set; }            // *종목 코드 (8자리)
        public string name { get; set; }            // 종목 이름
        public string type { get; set; }            // 시장 구분(KOSPI, KOSDAQ, KONEX)
        public double count { get; set; }           // 상장 주식 수

        public Stock()
        {
            code = "";
            name = "";
            type = "";
            coount = 0;
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("Stock Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("code: {0}", code);
            Console.WriteLine("name: {0}", name);
            Console.WriteLine("type: {0}", type);
            Console.WriteLine("coount: {0}", coount);
            Console.WriteLine("==================================================");
        }
    }
}