using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Events
{
    public enum EventType
    {
        Default,
        LinkedNavigation,
        Status,
        BacktestRequest,
        OptimizationRequest,
        Trade,
        Result
    }
}
