using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Framework.Data.Common
{
    public class DarkLightOrder
    {
        public DateTime Time { get; set; }
        public string Symbol { get; set; }
        public string Side { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
        public long Id { get; set; }
    }
}
