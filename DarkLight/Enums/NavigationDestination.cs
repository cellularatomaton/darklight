using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Events
{
    public enum NavigationDestination
    {
        Default,
        BacktestBrowser,
        BacktestLauncher,
        BacktestModule,
        BacktestStatus,
        Error,
        EventPublisher,
        Fills,
        Indicators,
        LiveTradingModule,
        LiveTradingPorfolios,
        Messages,
        OptimizationModule,
        OptimizationScheduler,
        Orders,
        ParametricRange,
        Positions,
        Response,
        ResponseConfiguration,
        ResponseSelection,
        Results,
        Statistics,
        TemporalRange,
        TickData,
        TimeSeries,
    }
}
