﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Common.Models;
using DarkLight.Enums;

namespace DarkLight.Backtest.Models
{
    public class ResponseSessionRecord
    {
        public string GUID { get; set; }
        public TradeMode Mode { get; set; }

        //stats
        public bool IsActive { get; set; }        
        public DateTime CreateDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }        
        public int NumTrades { get; set; }
        public double PNL { get; set; }
        public double WinLossRatio { get; set; }

    }
}
