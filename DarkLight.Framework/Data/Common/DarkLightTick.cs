using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Framework.Data.Common
{
    public class DarkLightTick
    {
        public DateTime Time { get; set; }
        public string Sym { get; set; }
        public int Trade { get; set; }
        public int TSize { get; set; }
        public int Bid { get; set; }
        public int Ask { get; set; }
        public int BSize { get; set; }
        public int ASize { get; set; }
        public string TExch { get; set; }
        public string BidExch { get; set; }
        public string AskExch { get; set; }
    }
}
