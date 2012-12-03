using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DarkLight.Backtest.Models;
using DarkLight.Common.Models;
using DarkLight.Enums;
using DarkLight.Utilities;

namespace DarkLight.Repositories
{
    class MockBacktestRepository : IBacktestRepository
    {
        public List<BacktestGroupRecord> GetBacktestGroupRecords(string query)
        {
            //TODO: implement Mongo query
            return MockUtilities.GenerateBacktestGroupRecords(20);
        }

        public List<ResponseSessionRecord> GetBacktestRecords(string backtestGroup, string query)
        {
            //TODO: implement Mongo query
            return MockUtilities.GenerateResponseSessionRecords(TradeMode.Backtest, 20);
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
