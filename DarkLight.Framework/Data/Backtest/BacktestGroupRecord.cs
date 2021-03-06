﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Framework.Data.Backtest
{
    public class BacktestGroupRecord
    {
        public string GUID { get; set; }
        public DateTime CreateDate { get; set; }
        public int NumBacktests { get; set; }
        public double MaxPNL { get; set; }
        public double MinPNL { get; set; }
    }
}
