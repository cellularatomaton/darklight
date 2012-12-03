using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Common.Models;
using DarkLight.Utilities;

namespace DarkLight.Events
{
    public class TradeEvent : DarkLightEvent
    {
        public TradeEventType Type { get; set; }

        public DarkLightFill Fill { get; set; }
        public DarkLightOrder Order { get; set; }
        public DarkLightPosition Position { get; set; }
        public DarkLightTick Tick { get; set; }
        public string Message { get; set; }

        public TradeEvent()
        {
            EventType = EventType.Trade;
        }
    }
}
