using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Backtest.Models;

namespace DarkLight.Events
{
    public class StatusEvent : ServiceEventBase
    {
        public StatusType StatusType { get; set; }

        public string Message { get; set; }
        public BacktestProgressModel[] ProgressModels { get; set; }
    }
}
