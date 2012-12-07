using System.Collections.Generic;
using Caliburn.Micro;
using DarkLight.Backtest.Models;
using DarkLight.Common.Models;

namespace DarkLight.Repositories
{
    public interface IBacktestRepository
    {
        List<BacktestGroupRecord> GetBacktestGroupRecords(string query);
        List<ResponseSessionRecord> GetBacktestRecords(BacktestGroupRecord backtestGroupRecord, string query);

        List<DarkLightFill> GetBacktestFills(string backtestID);
        List<string> GetBacktestMessages(string backtestID);
        List<DarkLightOrder> GetBacktestOrders(string backtestID);
        List<DarkLightPosition> GetBacktestPositions(string backtestID);
        List<string> GetBacktestResults(string backtestID);
        List<string> GetBacktestStatistics(string backtestID);
        List<DarkLightTick> GetBacktestTicks(string backtestID);
        List<string> GetBacktestTimeseries(string backtestID);
    }
}
