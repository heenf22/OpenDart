using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace OpenStock
{
    class Program
    {
        static void Main(string[] args)
        {
            bool result = StockService.Instance.initialize(@"/home/lgh/project/OpenDart/OpenStock/data");
            System.Console.WriteLine(">> initialize result: {0}", result);
            // StockService.Instance.importCsv(@"/home/lgh/project/OpenDart/OpenStock/data/data_1834_20210902.csv");
            StockService.Instance.importOpenDart();
            StockService.Instance.save();
            // test_read_csv();
        }

        static void test_read_csv()
        {
            int counter = 0;  
            string line;
            Dictionary<string, Stock> stocks = new Dictionary<string, Stock>();
            
            // 전체선택, 종목코드, 종목명, 시장구분, 소속부, 종가, 대비, 등락률, 시가, 고가, 저가, 거래량, 거래대금, 시가총액, 상장주식수
            // System.IO.StreamReader file = new System.IO.StreamReader(@"/home/lgh/project/OpenDart/OpenStock/data/data_1834_20210902.csv", Encoding.UTF8);
            System.IO.StreamReader file = new System.IO.StreamReader(@"/home/lgh/project/OpenDart/OpenStock/data/temp_utf8.csv");
            if ((line = file.ReadLine()) != null)
            {
                // 첫 라인은 컬럼 정보임
                while((line = file.ReadLine()) != null)  
                {
                    // string[] columns = line.Split(',');
                    // string euckr = Encoding.GetEncoding("euc-kr").GetString(
                    //                 Encoding.Convert(
                    //                 Encoding.UTF8,
                    //                 Encoding.GetEncoding("euc-kr"),
                    //                 Encoding.UTF8.GetBytes(line)));
                    // foreach (string e in columns)
                    // {
                    //     System.Console.WriteLine(">> {0}", e);
                    // }

                    Stock stock = new Stock();
                    stock.initialize(line.Replace("\"", ""));
                    // stock.reload();
                    stock.displayConsole();
                    stocks.Add(stock.Code, stock);

                    System.Console.WriteLine(line);
                    counter++;
                }
            }
            
            file.Close();  
            System.Console.WriteLine("There were {0} lines.", counter);  
            // Suspend the screen.  
            System.Console.ReadLine();
        }
    }
}
