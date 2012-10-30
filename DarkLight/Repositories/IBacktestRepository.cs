using System.Collections.Generic;
using Caliburn.Micro;
using DarkLight.Backtest.Models;

namespace DarkLight.Repositories
{
    public interface IBacktestRepository
    {
        List<BacktestGroupRecord> GetBacktestGroupRecords(string query);
        List<BacktestRecord> GetBacktestRecords(string backtestGroup, string query);

        List<string> GetBacktestFills(string backtestID);
        List<string> GetBacktestMessages(string backtestID); 
        List<string> GetBacktestOrders(string backtestID);
        List<string> GetBacktestPositions(string backtestID);
        List<string> GetBacktestResults(string backtestID);
        List<string> GetBacktestStatistics(string backtestID);
        List<string> GetBacktestTicks(string backtestID);
        List<string> GetBacktestTimeseries(string backtestID);
    }
}
