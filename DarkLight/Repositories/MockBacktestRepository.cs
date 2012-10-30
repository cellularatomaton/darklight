using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DarkLight.Backtest.Models;

namespace DarkLight.Repositories
{
    class MockBacktestRepository : IBacktestRepository
    {
        public List<BacktestGroupRecord> GetBacktestGroupRecords(string query)
        {
            var groupRecords = new List<BacktestGroupRecord>();

            //TODO: implement Mongo query
            for (int i = 0; i < 20; i++)
            {
                string description = query + " " + i.ToString();
                groupRecords.Add(new BacktestGroupRecord{Description = description});
            }

            return groupRecords;
        }

        public List<BacktestRecord> GetBacktestRecords(string backtestGroup, string query)
        {
            var backtestRecords = new List<BacktestRecord>();

            //TODO: implement Mongo query
            for (int i = 0; i < 20; i++)
            {
                string description = backtestGroup +  ": " + query + " " + i.ToString();
                backtestRecords.Add(new BacktestRecord{Description = description});
            }

            return backtestRecords;
        }


        public List<string> GetBacktestFills(string backtestID)
        {
            return new List<string>();
        }

        public List<string> GetBacktestMessages(string backtestID)
        {
            return new List<string>();
        }

        public List<string> GetBacktestOrders(string backtestID)
        {
            return new List<string>();
        }

        public List<string> GetBacktestPositions(string backtestID)
        {
            return new List<string>();
        }

        public List<string> GetBacktestResults(string backtestID)
        {
            return new List<string>();
        }

        public List<string> GetBacktestStatistics(string backtestID)
        {
            return new List<string>();
        }

        public List<string> GetBacktestTicks(string backtestID)
        {
            return new List<string>();
        }

        public List<string> GetBacktestTimeseries(string backtestID)
        {
            return new List<string>();
        }    
    }
}
