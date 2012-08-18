using System;
using System.Collections.Generic;
using System.Text;

namespace DarkLight.Utilities
{
 
    public enum PlayTo
    {
        LastPlayTo,
        OneSec = 1,
        ThirtySec = 30,
        OneMin = 10,
        FiveMin = 50,
        TenMin = 100,
        HalfHour = 300,
        Hour = 1000,
        TwoHour,
        FourHour,
        Custom,
        End,
    }

    public enum DispatchableType
    {
        Status,
        Message,
        Tick,
        Fill,
        Order,
        Position,
        Plot,
        Indicator,
    }

    public enum EngineType
    {
        Live,
        Backtest,
    }
}
