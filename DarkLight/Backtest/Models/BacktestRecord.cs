using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Backtest.Models
{
    public class BacktestRecord
    {
        public string Description { get; set; }
        public string GUID { get; set; }
        public DateTime CreateDate { get; set; }
        public int NumTrades { get; set; }
        public double PNL { get; set; }
        public double WinLossRatio { get; set; }
    }
}
