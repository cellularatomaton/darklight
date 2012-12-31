using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Framework.Data.Backtest;
using DarkLight.Framework.Enums;

namespace DarkLight.Framework.Events
{
    public class StatusEvent : DarkLightEvent
    {
        public StatusType StatusType { get; set; }
        public BacktestProgressModel[] ProgressModels { get; set; }
        public int NumBacktestsComplete { get; set; }
        public int NumBacktests { get; set; }

        public StatusEvent()
        {
            EventType = EventType.Status;
        }
    }
}
