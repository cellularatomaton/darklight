using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Customizations;
using DarkLight.Enums;
using DarkLight.Services;

namespace DarkLight.Events
{
    public class BacktestRequestEvent : DarkLightEvent
    {
        public ServiceAction ActionType;
        public DarkLightResponse Response;
        public IHistDataService HistDataService;

        public BacktestRequestEvent()
        {
            EventType = EventType.BacktestRequest;
        }
    }
}
