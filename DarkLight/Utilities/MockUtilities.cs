using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Backtest.Models;
using DarkLight.Common.Models;
using DarkLight.Enums;

namespace DarkLight.Utilities
{
    public static class MockUtilities
    {
        private static string[] tickers = new string[]
                                              {
                                                  "ES", "NQ", "YM", "IBM", "AAPL", "MSFT", "JPM", "WFC", "LOW", "HD", "KO",
                                                  "PEP", "HBC", "TGT", "SPY", "GLD", "IYR"
                                              };

        private static string[] _responseTypes = new string[] {"Momentum", "Pairs", "Allocator"};

        public static List<DarkLightFill> GenerateFills(string backtestid, int numFills)
        {
            var fills = new List<DarkLightFill>();
            Random random = new Random();
            string ticker = tickers[random.Next(1, tickers.Length + 1) - 1];
            var date = DateTime.Now.AddDays(-20);
            int id = random.Next(10000, 20000);
            double maxprice = 85.0;
            double minprice = 80.0;

            for (int i = 0; i < numFills; i++)
            {
                fills.Add(new DarkLightFill
                              {
                                  Time = date.AddMinutes(i).AddSeconds(random.Next(0, 59)),
                                  Symbol = ticker,
                                  Side = random.NextDouble() > 0.5 ? "Long" : "Short",
                                  Size = random.Next(1, 50),
                                  Price = Math.Round(random.NextDouble()*(maxprice - minprice) + minprice, 2),
                                  Id = id++
                              });
            }

            return fills;
        }

        public static List<DarkLightOrder> GenerateOrders(string backtestid, int numFills)
        {
            var orders = new List<DarkLightOrder>();
            Random random = new Random();
            string ticker = tickers[random.Next(1, tickers.Length + 1) - 1];
            var date = DateTime.Now.AddDays(-20);
            int id = random.Next(10000, 20000);
            double maxprice = 85.0;
            double minprice = 80.0;
            for (int i = 0; i < numFills; i++)
            {
                orders.Add(new DarkLightOrder
                               {
                                   Time = date.AddMinutes(i).AddSeconds(random.Next(0, 59)),
                                   Symbol = ticker,
                                   Side = random.NextDouble() > 0.5 ? "Long" : "Short",
                                   Size = random.Next(1, 50),
                                   Price = Math.Round(random.NextDouble()*(maxprice - minprice) + minprice, 2),
                                   Id = id++
                               });
            }

            return orders;
        }

        public static List<DarkLightPosition> GeneratePositions(string backtestid, int numFills)
        {
            var positions = new List<DarkLightPosition>();
            Random random = new Random();
            string ticker = tickers[random.Next(1, tickers.Length + 1) - 1];
            var date = DateTime.Now.AddDays(-20);
            double maxprice = 85.0;
            double minprice = 80.0;
            for (int i = 0; i < 20; i++)
            {
                positions.Add(new DarkLightPosition
                                  {
                                      Time = date.AddMinutes(i).AddSeconds(random.Next(0, 59)),
                                      Symbol = ticker,
                                      Side = random.NextDouble() > 0.5 ? "Long" : "Short",
                                      Size = random.Next(1, 50),
                                      AvgPrice = Math.Round(random.NextDouble()*(maxprice - minprice) + minprice, 2),
                                      Profit = Math.Round(random.NextDouble()*100, 2),
                                      Points = Math.Round(random.NextDouble()*100, 2)
                                  });
            }

            return positions;
        }

        public static List<DarkLightTick> GenerateTicks(string backtestid, int numFills)
        {
            var ticks = new List<DarkLightTick>();
            Random random = new Random();
            string ticker = tickers[random.Next(1, tickers.Length + 1) - 1];
            var date = DateTime.Now.AddDays(-20);

            for (int i = 0; i < numFills; i++)
            {
                ticks.Add(new DarkLightTick
                              {
                                  Time = date.AddMinutes(i).AddSeconds(random.Next(0, 59)),
                                  Sym = ticker,
                                  Trade = random.Next(1, 50),
                                  TSize = random.Next(1, 50),
                                  Bid = random.Next(1, 50),
                                  Ask = random.Next(1, 50),
                                  BSize = random.Next(1, 50),
                                  ASize = random.Next(1, 50),
                                  TExch = "CME",
                                  BidExch = "--",
                                  AskExch = "--",
                              });
            }

            return ticks;
        }

        public static List<BacktestGroupRecord> GenerateBacktestGroupRecords(int numGroups)
        {
            var groupRecords = new List<BacktestGroupRecord>();
            Random random = new Random();
            int iRandomResponseIndex = random.Next(1, _responseTypes.Length + 1) - 1;
            var responseType = _responseTypes[iRandomResponseIndex];

            for (int i = 0; i < numGroups; i++)
            {
                int xmaMax = random.Next(10, 20);
                int intervalMin = random.Next(1, 3)*5 + 5;
                int intervalMax = random.Next(2, 3) * 5 + intervalMin;
                int daysBack = -random.Next(25, 35);
                int daysLong = random.Next(1,2)*4 + 3;
                var startDate = DateTime.Now.AddDays(daysBack);
                var endDate = startDate.AddDays(daysLong);

                string parameterSpace = "XMACoefficient[0," + xmaMax + "]|IntervalSize[" + intervalMin + "," +
                                         intervalMax + "]|" +
                                        startDate.ToString("MM/dd/yyyy") + "|" +
                                        endDate.ToString("MM/dd/yyyy");
                groupRecords.Add(new BacktestGroupRecord
                                     {
                                         ResponseType = responseType,
                                         ParameterSpace = parameterSpace,
                                         GUID = responseType + "|" + parameterSpace, //Guid.NewGuid().ToString(),                                         
                                         CreateDate = DateTime.Now.AddDays(-random.Next(1, 7)).AddSeconds(random.Next(0, 59)),
                                         NumBacktests = (xmaMax + 1) * (intervalMax - intervalMin) * daysLong,
                                         MaxPNL = Math.Round(100*random.NextDouble(), 2),
                                         MinPNL = Math.Round(-100*random.NextDouble(), 2)
                                     });
            }

            return groupRecords;
        }

        public static List<ResponseSessionRecord> GenerateResponseSessionRecords(TradeMode mode, int numRecords)
        {
            var backtestRecords = new List<ResponseSessionRecord>();

            Random random = new Random();
            int iRandomResponseIndex = random.Next(1, _responseTypes.Length + 1) - 1;
            var responseType = _responseTypes[iRandomResponseIndex];
            var date = DateTime.Now.AddDays(-20).AddMinutes(random.Next(1, 59)).AddSeconds(random.Next(1, 59));
            for (int i = 0; i < 20; i++)
            {
                double pnlSwitch = random.NextDouble() > 0.5 ? 1 : -1;
                DateTime tradeDate = date.AddDays(-random.Next(10, 100));
                string parameters = "XMACoefficient=" + random.Next(0, 50) + "|IntervalSize=" + random.Next(25, 75) +
                                    "|" +
                                    tradeDate.Date.ToString("MM/dd/yyy");
                backtestRecords.Add(new ResponseSessionRecord
                                        {
                                            Mode = mode,
                                            GUID = "B|" + responseType + "|" + parameters,
                                            ResponseType = responseType,
                                            Parameters = parameters,
                                            TradeDate = tradeDate,
                                            NumTrades = random.Next(5, 100),
                                            PNL = Math.Round(pnlSwitch*100*random.NextDouble(), 2),
                                            WinLossRatio = Math.Round(1 + random.NextDouble(), 2)
                                        });
            }

            return backtestRecords;
        }           

        public static BacktestGroupRecord ParseGroupGUID(string guid)
        {
            var record = new BacktestGroupRecord();
            var keyArray = guid.Split(new char[] { '|' });

            record.ResponseType = keyArray[0];

            string parameters = "";
            if (keyArray.Length >= 2)
                for (int i = 1; i < keyArray.Length; i++)
                {
                    parameters += keyArray[i] + "|";
                }
            record.ParameterSpace = parameters.Remove(parameters.Length - 1, 1);
            return record;            
        }
   
        public static ResponseSessionRecord ParseSessionGUID(string guid)
        {
            var record = new ResponseSessionRecord();
            var keyArray = guid.Split(new char[] { '|' });

            record.Mode = keyArray[0] == "B" ? TradeMode.Backtest : TradeMode.Live;
            record.ResponseType = keyArray[1];
            
            string parameters = "";
            if (keyArray.Length >= 3)
                for (int i = 2; i < keyArray.Length; i++)
                {
                    parameters += keyArray[i] + "|";
                }
            record.Parameters = parameters.Remove(parameters.Length - 1, 1);
            return record;
        }
    }
}
