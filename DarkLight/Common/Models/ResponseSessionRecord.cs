using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Enums;

namespace DarkLight.Backtest.Models
{
    public class ResponseSessionRecord
    {
        public TradeMode Mode { get; set; }
        public string GUID { get; set; }
        public string ResponseType { get; set; }
        public string Parameters { get; set; }
        public string Products { get; set; }
        public bool IsActive { get; set; }
        public DateTime TradeDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        //stats
        public int NumTrades { get; set; }
        public double PNL { get; set; }
        public double WinLossRatio { get; set; }

    }
}
