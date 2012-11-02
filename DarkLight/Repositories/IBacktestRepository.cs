using System.Collections.Generic;
using Caliburn.Micro;
using DarkLight.Backtest.Models;

namespace DarkLight.Repositories
{
    public interface IBacktestRepository
    {
        List<BacktestGroupRecord> GetBacktestGroupRecords(string query);
        List<BacktestRecord> GetBacktestRecords(string backtestGroup, string query);
    }
}
