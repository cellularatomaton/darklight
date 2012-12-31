using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Framework.Data.Common
{
    public class DarkLightPosition
    {
        public DateTime Time { get; set; }
        public string Symbol { get; set; }
        public string Side { get; set; }
        public int Size { get; set; }
        public double AvgPrice { get; set; }
        public double Profit { get; set; }
        public double Points { get; set; }
    }
}
