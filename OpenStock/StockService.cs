using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using OpenDart.Models;
using OpenDart.OpenDartClient;

namespace OpenStock
{
    public class StockService
    {
        private static volatile StockService instance;
        private static object syncRoot = new object();
        public static StockService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new StockService();
                        }
                    }
                }

                return instance;
            }
        }

        public static JsonSerializerOptions jsonSerializerOptionsCamelCasePretty { get; } = new JsonSerializerOptions
        {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // 첫글자 소문자 형태의 lowerCamelCase
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true                                // 문단 구분되어 보기 좋을 형태로 정렬
        };

        public static JsonSerializerOptions jsonSerializerOptionsCamelCase { get; } = new JsonSerializerOptions
        {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // 첫글자 소문자 형태의 lowerCamelCase
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = false                               // 문단 구분 없음
        };

        private Dictionary<string, Stock> stocks { get; set; }      // Stock Information (key = Stock Code)
        private string path { get; set; }                           // Stock file path (stocks.json)

        private StockService()
        {
            stocks = new Dictionary<string, Stock>();
        }

        public bool initialize(string path)
        {
            try
            {
                this.path = path + Path.DirectorySeparatorChar + "stocks.json";
                if (File.Exists(this.path))
                {
                    return load();
                }
                else
                {
                    return save();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("!!! Exception: {0}", e.Message);
            }

            return false;
        }

        public void clear()
        {
            stocks.Clear();
        }

        public bool importCsv(string csvPath)
        {
            StreamReader sr = new StreamReader(csvPath);
            try
            {
                string line = "";
                if ((line = sr.ReadLine()) != null)
                {
                    // 첫 라인은 컬럼 정보임
                    while((line = sr.ReadLine()) != null)  
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
                        if (isExist(stock))
                        {
                            update(stock);
                        }
                        else
                        {
                            add(stock);
                        }
                        // System.Console.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("!!! Exception: {0}", e.Message);
                return false;
            }
            finally
            {
                sr.Close();  
            }
            
            return true;            
        }

        public void importOpenDart()
        {
            System.Console.WriteLine(">> start... importOpenDart, stocks count: {0}", stocks.Count);
            foreach (KeyValuePair<string, Stock> e in stocks)
            {
                e.Value.displayConsole();

                OpenDartClient odc = new OpenDartClient("af02b784cd62f41d5601bea249119dac0890a123", @"/home/lgh/project/OpenDart/OpenStock/data");
                ResFnlttSinglAcntResult result = odc.REQ3_1_GET_FNLTT_SINGL_ACNT_INFO("00" + e.Value.Code, "2020", "11011");
                if (result != null)
                {
                    System.Console.WriteLine(">> result is not null... importOpenDart");
                    result.displayConsole();
                }
                else
                {
                    System.Console.WriteLine(">> result is null... importOpenDart");
                }

                break;
            }

            System.Console.WriteLine(">> end... importOpenDart");
        }

        public bool load()
        {
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.UTF8);
                if (sr != null)
                {
                    string json = sr.ReadToEnd();
                    sr.Close();

                    if (!string.IsNullOrEmpty(json))
                    {
                        stocks = JsonSerializer.Deserialize<Dictionary<string, Stock>>(json, jsonSerializerOptionsCamelCasePretty);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("!!! Exception: {0}", e.Message);
            }

            return false;
        }

        public bool save()
        {
            try
            {
                string json = JsonSerializer.Serialize<Dictionary<string, Stock>>(stocks, jsonSerializerOptionsCamelCasePretty);
                if (!string.IsNullOrEmpty(json))
                {
                    StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
                    if (sw != null)
                    {
                        sw.Write(json);
                        sw.Close();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("!!! Exception: {0}", e.Message);
            }

            return false;
        }

        public bool add(Stock obj)
        {
            try
            {
                stocks.Add(obj.Code, obj);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"!!! Exception: {e.Message}");
            }

            return false;
        }

        public bool update(Stock obj)
        {
            try
            {
                stocks[obj.Code] = obj;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("!!! Exception: {0}", e.Message);
            }

            return false;
        }

        public bool remove(Stock obj)
        {
            return stocks.Remove(obj.Code);
        }

        public bool isExist(Stock obj)
        {
            try
            {
                Stock find = stocks[obj.Code];
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine("!!! KeyNotFoundException: {0}", e.Message);
                return false;
            }

            return true;
        }

        public Stock getStock(string id)
        {
            try
            {
                return stocks[id];
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine("!!! KeyNotFoundException: {0}", e.Message);
            }
            
            return null;
        }

        public IEnumerable<Stock> getStocks()
        {
            return stocks.Values;
        } 

        public Stock findStockName(string name)
        {
            foreach (KeyValuePair<string, Stock> e in stocks)
            {
                if (e.Value.Name.ToLower().Contains(name.ToLower()))
                {
                    return e.Value;
                }
            }

            return null;
        }

        public Task<Stock[]> getStocksAsync(DateTime startDate)
        {
            return Task.FromResult(stocks.Values.ToArray());
        }

        public void displayConsole(bool isLocal = false)
        {
            foreach (KeyValuePair<string, Stock> e in stocks)
            {
                e.Value.displayConsole(isLocal);
            }
        }
    }
}