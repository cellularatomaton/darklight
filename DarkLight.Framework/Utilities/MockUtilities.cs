using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Framework.Data.Backtest;
using DarkLight.Framework.Data.Common;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Utilities;


namespace DarkLight.Framework.Utilities
{
    public static class MockUtilities
    {
        private static string[] tickers = new string[]{"ES", "NQ", "YM", "IBM", "AAPL", "MSFT", "JPM", "WFC", "LOW", "HD", "KO",
                                                       "PEP", "HBC", "TGT", "SPY", "GLD", "IYR" };

        private static string[] _responseTypes = new string[] {"Momentum", "Pairs", "Allocator"};

        public static List<DarkLightFill> GenerateFills(string backtestid, int numFills)
        {
            var fills = new List<DarkLightFill>();
            Random random = new Random();
            string ticker = tickers[random.Next(1, tickers.Length + 1) - 1];
            var date = Convert.ToDateTime(GetTradeDateFromGUID(backtestid));
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
            var date = Convert.ToDateTime(GetTradeDateFromGUID(backtestid));
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
            var date = Convert.ToDateTime(GetTradeDateFromGUID(backtestid));
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
            var date = Convert.ToDateTime(GetTradeDateFromGUID(backtestid));

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
                var paramSpaceList = new List<ConfigurationVariableSpace<double>>();
                paramSpaceList.Add(new ConfigurationVariableSpace<double>("XMACoefficient", 0, random.Next(10, 20), 1));
                int intervalMin = random.Next(1, 3) * 5 + 5;
                int intervalMax = random.Next(2, 3) * 5 + intervalMin;
                paramSpaceList.Add(new ConfigurationVariableSpace<double>("IntervalSize", intervalMin, intervalMax, 1));

                int daysBack = -random.Next(25, 35);
                int daysLong = random.Next(1,2)*4 + 3;
                var startDate = DateTime.Now.AddDays(daysBack);
                var endDate = startDate.AddDays(daysLong);
                var temporalSpace = new ConfigurationVariableSpace<DateTime>("TradeDate", startDate, endDate, 1);

                var configurationSpace = new ConfigurationSpace();
                configurationSpace.ParameterSpace = paramSpaceList;
                configurationSpace.TemporalSpace = temporalSpace;

                groupRecords.Add(new BacktestGroupRecord
                                     {
                                         GUID = CommonFunctions.GenerateBacktestGroupGUID(responseType, configurationSpace),                                                                                                                       
                                         CreateDate = DateTime.Now.AddDays(-random.Next(1, 7)).AddSeconds(random.Next(0, 59)),
                                         NumBacktests = configurationSpace.GetSpaceSize(),
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
                                            GUID = responseType + "|" + parameters,
                                            Mode = mode,                                            
                                            NumTrades = random.Next(5, 100),
                                            PNL = Math.Round(pnlSwitch*100*random.NextDouble(), 2),
                                            WinLossRatio = Math.Round(1 + random.NextDouble(), 2)
                                        });
            }

            return backtestRecords;
        }

        public static List<ResponseSessionRecord> GenerateResponseSessionRecords(string backtestGroupGUID)
        {
            var backtestRecords = new List<ResponseSessionRecord>();
            Random random = new Random();
            DateTime createDate = DateTime.Now.AddDays(-random.Next(1, 7)).AddSeconds(random.Next(0, 59)); //won't match grouprecord since not embedded in string

            string[] responseConfigString = backtestGroupGUID.Split(new[] { "|" }, 2, StringSplitOptions.None);
            string responseType = responseConfigString[0];
            string configString = responseConfigString[1];            
            var configSpace = CreateConfigurationSpace(configString);
            var paramSpace = configSpace.ParameterSpace;
            var temporalSpace = configSpace.TemporalSpace;
            int numTests = configSpace.GetSpaceSize();

            var expandedParamSpaceList = ExpandParameterSpaceList(paramSpace);
            var expandedDateSpace = ExpandTemporalSpace(temporalSpace);
            int numVariables = expandedParamSpaceList.Count + 1;
            var countList =  Enumerable.Range(0, paramSpace.Count()).Select(i => (paramSpace[i].Quantity)).ToList();
            countList.Add(expandedDateSpace.Count);

            int[] localIndexArr = new int[numVariables];                       

            //hackish dynamic nested for loops
            for (int n = 0; n < numTests; n++)
            {
                double pnlSwitch = random.NextDouble() > 0.5 ? 1 : -1;

                //create parameters
                string parameters = "";
                for (int i = 0; i < numVariables - 1; i++)
                {
                    int localConfigIndex = localIndexArr[i];
                    parameters += paramSpace[i].Name + "=" + expandedParamSpaceList[i][localConfigIndex].ToString() + "|";
                }
                parameters = parameters.Substring(0, parameters.Length - 1);

                int dateIndex = localIndexArr[numVariables - 1];
                DateTime tradeDate = expandedDateSpace[dateIndex];

                //create record
                backtestRecords.Add(new ResponseSessionRecord
                {
                    GUID = responseType + "|" + parameters + "|" + tradeDate.ToString("MM/dd/yyy"),
                    Mode = TradeMode.Backtest,                    
                    CreateDate = createDate,
                    NumTrades = random.Next(5, 100),
                    PNL = Math.Round(pnlSwitch * 100 * random.NextDouble(), 2),
                    WinLossRatio = Math.Round(1 + random.NextDouble(), 2)
                });

                //increment
                int variableIndex = numVariables - 1;
                while(variableIndex >= 0)
                {
                    if (localIndexArr[variableIndex] < countList[variableIndex] - 1)
                    {
                        localIndexArr[variableIndex]++;
                        break;
                    }

                    localIndexArr[variableIndex] = 0;
                    variableIndex--;
                }
            }

            return backtestRecords;
        }
        /*
        public static BacktestGroupRecord CreateBacktestGroupRecord(string guid)
        {
            var record = new BacktestGroupRecord();
            record.GUID = guid;

            var responseConfigArr = guid.Split(new[] { "|" }, 2, StringSplitOptions.None);

            record.ResponseType = responseConfigArr[0];
            record. = CreateConfigurationSpace(responseConfigArr[1]);

            return record;
        }
    */
        public static ConfigurationSpace CreateConfigurationSpace(string configString)
        {
            int indexLastTrade = configString.LastIndexOf("|");
            int indexFirstTrade = configString.Substring(0,indexLastTrade).LastIndexOf("|");
            var minDateString = configString.Substring(indexFirstTrade + 1, indexLastTrade - indexFirstTrade - 1);
            var maxDateString = configString.Substring(indexLastTrade + 1, configString.Length - indexLastTrade - 1);
            var minDate = Convert.ToDateTime(minDateString);
            var maxDate = Convert.ToDateTime(maxDateString);
            var temporalSpace = new ConfigurationVariableSpace<DateTime>("TradeDate", minDate, maxDate, 1);

            string paramString = configString.Substring(0, indexFirstTrade);
            var paramSpaceList = ParseParameterSpace(paramString);

            var configSpace = new ConfigurationSpace{ParameterSpace = paramSpaceList, TemporalSpace = temporalSpace};
           
            return configSpace;
        }

        public static List<ConfigurationVariableSpace<double>> ParseParameterSpace(string parameterSpace)
        {
            var list = new List<ConfigurationVariableSpace<double>>();
            var parameterStringArray = parameterSpace.Split(new char[] { '|' });

            foreach (var parameterString in parameterStringArray)
            {
                var parseArray = ParseSpace(parameterString);
                list.Add(new ConfigurationVariableSpace<double>(parseArray[0], Convert.ToDouble(parseArray[1]), Convert.ToDouble(parseArray[2]), 1));
            }

            return list;
        }
        
        public static List<ConfigurationVariableSpace<DateTime>> ParseTemporalSpace(string temporalSpace)
        {
            var list = new List<ConfigurationVariableSpace<DateTime>>();
            var temporalStringArray = temporalSpace.Split(new char[] { '|' });

            foreach (var temporalString in temporalStringArray)
            {
                var parseArray = ParseSpace(temporalString);
                list.Add(new ConfigurationVariableSpace<DateTime>(parseArray[0], Convert.ToDateTime(parseArray[1]), Convert.ToDateTime(parseArray[2]), 1));
            }

            return list;
        }
        
        public static string GetResponseFromGUID(string guid)
        {
            return guid.Split(new[] { "|" }, 2, StringSplitOptions.None)[0];
        }

        public static string GetParametersFromGUID(string guid)
        {
            string configString = guid.Split(new[] { "|" }, 2, StringSplitOptions.None)[1];
            int tradeDateIndex = configString.LastIndexOf("|");            
            return configString.Substring(0, tradeDateIndex - 1);
        }

        public static string GetTradeDateFromGUID(string guid)
        {
            string configString = guid.Split(new[] {"|"}, 2, StringSplitOptions.None)[1];
            int tradeDateIndex = configString.LastIndexOf("|");
            return configString.Substring(tradeDateIndex + 1, configString.Length - tradeDateIndex - 1);
        }

        static string[] ParseSpace(string variableSpace)
        {
            var varA = variableSpace.Split(new char[] { '[' });
            var varB = varA[1].Split(new char[] { ']' });
            var varC = varB[0].Split(new char[] { ',' });

            return new[] { varA[0].Replace("=",""), varC[0], varC[1] };
        } 

        static List<double[]> ExpandParameterSpaceList(List<ConfigurationVariableSpace<double>> paramSpace)
        {
            var expandedParamList = new List<double[]>();

            foreach (var param in paramSpace)
            {
                var valueList = Enumerable.Range(0, param.Quantity).Select(i => (i * param.Step + param.Min)).ToArray();
                expandedParamList.Add(valueList);
            }           

            return expandedParamList;
        }

        static List<DateTime> ExpandTemporalSpace(ConfigurationVariableSpace<DateTime> temporalSpace)
        {
            var expandedDateList = new List<DateTime>();

            DateTime currentDate = temporalSpace.Min;
            while (currentDate <= temporalSpace.Max)
            {
                //if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                    expandedDateList.Add(currentDate);

                currentDate = currentDate.AddDays(1);
            }

            return expandedDateList;
        }
    }
}
