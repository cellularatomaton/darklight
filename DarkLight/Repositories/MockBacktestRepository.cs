using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DarkLight.Backtest.Models;
using DarkLight.Common.Models;
using DarkLight.Utilities;

namespace DarkLight.Repositories
{
    class MockBacktestRepository : IBacktestRepository
    {
        public List<BacktestGroupRecord> GetBacktestGroupRecords(string query)
        {
            var groupRecords = new List<BacktestGroupRecord>();

            //TODO: implement Mongo query
            Random random = new Random();
            var date = DateTime.Now.AddDays(-20);

            for (int i = 0; i < 20; i++)
            {
                string description = query + " " + i.ToString();
                groupRecords.Add(new BacktestGroupRecord
                                     {
                                         Description = description,
                                         GUID = Guid.NewGuid().ToString(),
                                         CreateDate = date.AddDays(-random.Next(10,100)).AddSeconds(random.Next(0,59)),
                                         NumBacktests = random.Next(5,100),
                                         MaxPNL =  Math.Round(100 * random.NextDouble(),2),
                                         MinPNL =  Math.Round(-100 * random.NextDouble(),2)
                                     });
            }

            return groupRecords;
        }

        public List<BacktestRecord> GetBacktestRecords(string backtestGroup, string query)
        {
            var backtestRecords = new List<BacktestRecord>();

            //TODO: implement Mongo query
            Random random = new Random();
            var date = DateTime.Now.AddDays(-20);
            for (int i = 0; i < 20; i++)
            {
                double pnlSwitch = random.NextDouble() > 0.5 ? 1 : -1;
                string description = backtestGroup +  ": " + query + " " + i.ToString();
                backtestRecords.Add(new BacktestRecord{
                    Description = description,
                    GUID = Guid.NewGuid().ToString(),
                    CreateDate = date.AddDays(-random.Next(10, 100)).AddSeconds(random.Next(0, 59)),
                    NumTrades = random.Next(5, 100),
                    PNL = Math.Round(pnlSwitch * 100 * random.NextDouble(), 2),
                    WinLossRatio = Math.Round(1 + random.NextDouble(), 2)                
                });
            }

            return backtestRecords;
        }

        public List<DarkLightFill> GetBacktestFills(string backtestID)
        {
            //TODO: implement Mongo query
            return MockUtilities.GenerateFills(backtestID, 20);
        }

        public List<string> GetBacktestMessages(string backtestID)
        {
            return new List<string>();
        }

        public List<DarkLightOrder> GetBacktestOrders(string backtestID)
        {
            //TODO: implement Mongo query
            return MockUtilities.GenerateOrders(backtestID, 20);
        }

        public List<DarkLightPosition> GetBacktestPositions(string backtestID)
        {
            //TODO: implement Mongo query
            return MockUtilities.GeneratePositions(backtestID, 20);
        }

        public List<string> GetBacktestResults(string backtestID)
        {
            return new List<string>();
        }

        public List<string> GetBacktestStatistics(string backtestID)
        {
            return new List<string>();
        }

        public List<DarkLightTick> GetBacktestTicks(string backtestID)
        {
            //TODO: implement Mongo query
            return MockUtilities.GenerateTicks(backtestID, 20);
        }

        public List<string> GetBacktestTimeseries(string backtestID)
        {
            return new List<string>();
        }    
    }
}
