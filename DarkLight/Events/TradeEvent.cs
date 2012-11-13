using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Common.Models;

namespace DarkLight.Events
{
    public class TradeEvent : ServiceEventBase
    {
        public TradeEventType Type { get; set; }

        public DarkLightFill Fill { get; set; }
        public DarkLightOrder Order { get; set; }
        public DarkLightTick Tick { get; set; }
        public string Message { get; set; }
    }
}
