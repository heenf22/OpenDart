using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Appraiser.Models
{
    public class Stock
    {
        public string Code { get; set; }        // 종목 코드
        public string Name { get; set; }        // 종목 이름
        public StockType Type { get; set; }     // 시장 구분 (KOSPI, KOSDAQ, KONEX)
        public int Earnings { get; set; }       // 발행주식 수

        public Stock()
        {

        }
    }
}
