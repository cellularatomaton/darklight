using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Common.Models
{
    public class DarkLightFill
    {
        public DateTime Time { get; set; }
        public string Symbol { get; set; }
        public string Side { get; set; }
        public int Size { get; set; }
        public double Price { get; set; }
        public long Id { get; set; }
    }
}
