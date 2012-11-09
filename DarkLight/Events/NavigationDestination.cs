using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Events
{
    public enum NavigationDestination
    {
        Default,
        BacktestModule,
        BacktestLauncher,
        BacktestBrowser,
        OptimizationModule,
        LiveTradingModule,
        Fills,
        Indicators,
        Messages,
        Orders,
        Positions,
        Results,
        Response,
        Statistics,
        TickData,
        TimeSeries,
        EventPublisher,
        ResponseSelection,
        ResponseConfiguration,
        ParametricRange,
        TemporalRange,
        BacktestStatus
    }
}
