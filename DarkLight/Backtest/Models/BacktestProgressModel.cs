using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkLight.Backtest.Models
{
    public class BacktestProgressModel
    {
        public string Slot { get; set; }
        public string BacktestID { get; set; }
        public double ProgressValue { get; set; }

    }
}
