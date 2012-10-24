using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DarkLight.Backtest.Models;

namespace DarkLight.Repositories
{
    class DarklightBacktestRepository : IBacktestRepository
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
    }
}
