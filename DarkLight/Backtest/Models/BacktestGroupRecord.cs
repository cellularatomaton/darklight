using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Backtest.Models
{
    public class BacktestGroupRecord
    {
        public string Description { get; set; }
        public string GUID { get; set; }
        public DateTime CreateDate { get; set; }
        public int NumBacktests { get; set; }
        public float MaxPNL { get; set; }
        public float MinPNL { get; set; }    
    }
}
