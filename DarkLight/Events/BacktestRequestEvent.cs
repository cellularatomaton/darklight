using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Customizations;
using DarkLight.Services;

namespace DarkLight.Events
{
    public class BacktestRequestEvent : DarkLightEvent
    {
        public DarkLightResponse Response;
        public IHistDataService HistDataService;

        public BacktestRequestEvent()
        {
            EventType = EventType.BacktestRequest;
        }
    }
}
