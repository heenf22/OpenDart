using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenStock
{
    public class Stock
    {
        public string Code { get; set; }            // *종목 코드 (8자리)
        public string Name { get; set; }            // 종목 이름
        public string Type { get; set; }            // 시장 구분(KOSPI, KOSDAQ, KONEX)
        public double LastPrise { get; set; }       // 종가
        public double TransactionVolume { get; set; }   // 거래량
        public double TransactionPrice { get; set; }    // 거래대금
        public double LastStockCount { get; set; }      // 상장주식수
        public double LastProfit1 { get; set; }         // 영엽이익(직전년도)
        public double LastProfit2 { get; set; }         // 당기순익(직전년도)
        public double LastProfit3 { get; set; }         // 평균순익(직전년도) = 영엽이익(LastProfit1) + 당기순익(LastProfit2) / 2
        //---
        public double PresentTotalStockPrise { get; set; }  // 현재 시가총액 = 상장주식수(LastStockCount) x 종가(LastPrise)
        public double FutureTotalStockPrise { get; set; }   // 적정 시가총액 = 영업이익(LastProfit1) x 적정 PER(UserPER)
        public double LastPER { get; set; }                 // 현재 PER(평균) = 현재 시가총액(PresentTotalStockPrise) / 평균순익(LastProfit3)
        public double UserPER { get; set; }                 // 적정 PER = 투자자가 결정
        public double EPS1 { get; set; }                // EPS(영업이익) = 영업이익(LastProfit1) / 상장주식수(LastStockCount)
        public double EPS2 { get; set; }                // EPS(당기순익) = 당기순익(LastProfit1) / 상장주식수(LastStockCount)
        public double EPS3 { get; set; }                // EPS(평균순익) = 평균순익(LastProfit1) / 상장주식수(LastStockCount)
        //---
        public double FutureStockPrice1 { get; set; }   // 미래주가(영업이익) = EPS(영업이익) x UserPER
        public double FutureStockPrice2 { get; set; }   // 미래주가(당기순익) = EPS(당기순익) x UserPER
        public double FutureStockPrice3 { get; set; }   // 미래주가(평균순익) = EPS(평균순익) x UserPER
        //---
        public bool ValuationBuy1 { get; set; }         // 기업평가(영업이익), true: 매수(종가 < 미래주가(영업이익)), false: 고평가
        public double ValuationPrise1 { get; set; }     // 주가차이(영업이익) = 미래주가(영업이익) - 종가
        public bool ValuationBuy2 { get; set; }         // 기업평가(당기순익), true: 매수(종가 < 미래주가(당기순익)), false: 고평가
        public double ValuationPrise2 { get; set; }     // 주가차이(당기순익) = 미래주가(당기순익) - 종가
        public bool ValuationBuy3 { get; set; }         // 기업평가(평균순익), true: 매수(종가 < 미래주가(평균순익)), false: 고평가
        public double ValuationPrise3 { get; set; }     // 주가차이(평균순익) = 미래주가(평균순익) - 종가
        //---
        public double BuyPrise { get; set; }            // 매입가(평균)
        public double BuyStockCount { get; set; }       // 보유주식수
        public double BuyProfit { get; set; }           // 평가손익
        public double BuyProfitRatio { get; set; }      // 수익율
        //---
        public DateTime LastUpdateDT { get; set; }      // 마지막 업데이트 시간

        public Stock()
        {
            Code = "";
            Name = "";
            Type = "";
            LastPrise = 0;
            TransactionVolume = 0;
            TransactionPrice = 0;
            LastStockCount = 0;
            LastProfit1 = 0;
            LastProfit2 = 0;
            LastProfit3 = 0;

            PresentTotalStockPrise = 0;
            FutureTotalStockPrise = 0;
            LastPER = 0;
            UserPER = 0;
            EPS1 = 0;
            EPS2 = 0;
            EPS3 = 0;

            FutureStockPrice1 = 0;
            FutureStockPrice2 = 0;
            FutureStockPrice3 = 0;

            ValuationBuy1 = false;
            ValuationPrise1 = 0;
            ValuationBuy2 = false;
            ValuationPrise2 = 0;
            ValuationBuy3 = false;
            ValuationPrise3 = 0;

            BuyPrise = 0;
            BuyStockCount = 0;
            BuyProfit = 0;
            BuyProfitRatio = 0;

            LastUpdateDT = DateTime.UtcNow;
        }

        public void displayConsole()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("Stock Information");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("code: {0}", code);
            Console.WriteLine("name: {0}", name);
            Console.WriteLine("type: {0}", type);
            Console.WriteLine("LastPrise: {0}", LastPrise);
            Console.WriteLine("TransactionVolume: {0}", TransactionVolume);
            Console.WriteLine("TransactionPrice: {0}", TransactionPrice);
            Console.WriteLine("LastStockCount: {0}", LastStockCount);
            Console.WriteLine("LastProfit1: {0}", LastProfit1);
            Console.WriteLine("LastProfit2: {0}", LastProfit2);
            Console.WriteLine("LastProfit3: {0}", LastProfit3);
            Console.WriteLine("PresentTotalStockPrise: {0}", PresentTotalStockPrise);
            Console.WriteLine("FutureTotalStockPrise: {0}", FutureTotalStockPrise);
            Console.WriteLine("LastPER: {0}", LastPER);
            Console.WriteLine("UserPER: {0}", UserPER);
            Console.WriteLine("EPS1: {0}", EPS1);
            Console.WriteLine("EPS2: {0}", EPS2);
            Console.WriteLine("EPS3: {0}", EPS3);
            Console.WriteLine("FutureStockPrice1: {0}", FutureStockPrice1);
            Console.WriteLine("FutureStockPrice2: {0}", FutureStockPrice2);
            Console.WriteLine("FutureStockPrice3: {0}", FutureStockPrice3);
            Console.WriteLine("ValuationBuy1: {0}", ValuationBuy1);
            Console.WriteLine("ValuationPrise1: {0}", ValuationPrise1);
            Console.WriteLine("ValuationBuy2: {0}", ValuationBuy2);
            Console.WriteLine("ValuationPrise2: {0}", ValuationPrise2);
            Console.WriteLine("ValuationBuy3: {0}", ValuationBuy3);
            Console.WriteLine("ValuationPrise3: {0}", ValuationPrise3);
            Console.WriteLine("BuyPrise: {0}", BuyPrise);
            Console.WriteLine("BuyStockCount: {0}", BuyStockCount);
            Console.WriteLine("BuyProfit: {0}", BuyProfit);
            Console.WriteLine("BuyProfitRatio: {0}", BuyProfitRatio);
            Console.WriteLine("LastUpdateDT: {0}", LastUpdateDT);
            Console.WriteLine("==================================================");
        }
    }
}