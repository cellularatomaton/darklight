using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DarkLight.Framework.Data.Backtest;
using DarkLight.Framework.Data.Common;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Interfaces.Repository;

namespace DarkLight.Framework.Utilities
{
    public class MockBacktestRepository : IBacktestRepository
    {
        public List<BacktestGroupRecord> GetBacktestGroupRecords(string query)
        {
            //TODO: implement Mongo query
            return MockUtilities.GenerateBacktestGroupRecords(20);
        }

        public List<ResponseSessionRecord> GetBacktestRecords(BacktestGroupRecord backtestGroup, string query)
        {
            //TODO: implement Mongo query
            var filteredList = new List<ResponseSessionRecord>();
            var completeList = MockUtilities.GenerateResponseSessionRecords(backtestGroup.GUID);
            foreach (var record in completeList)
            {
                if (record.GUID.Contains(query))
                    filteredList.Add(record);
            }

            return filteredList;

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
