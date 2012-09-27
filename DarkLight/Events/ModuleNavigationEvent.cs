using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Events
{
    public enum Destination
    {
        Backtest,
        Optimization,
        LiveTrading,
    }

    public class NavigationEvent
    {
        public Destination NavigateTo { get; set; }
    }
}
