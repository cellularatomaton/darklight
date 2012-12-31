using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Framework.Data.Common;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Interfaces.Services;

namespace DarkLight.Framework.Events
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
