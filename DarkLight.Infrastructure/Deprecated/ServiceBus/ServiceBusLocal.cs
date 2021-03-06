﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkLight.Framework.Data.Common;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Adapters;
using DarkLight.Framework.Interfaces.Services;
using com.espertech.esper.client;
using EventType = DarkLight.Framework.Enums.EventType;

namespace DarkLight.Infrastructure.ServiceBus
{
    public class ServiceBusLocal : IMediatorAdapter, IBacktestAdapter
    {        
        //IBacktestAdapter
        public Action<IHistDataService, DarkLightResponse> OnRunBacktest { get; set; }
        public Action<string> OnPauseBacktest { get; set; }
        public Action<string> OnResumeBacktest { get; set; }
        public Action<string> OnCancelBacktest { get; set; }
        
        //IMediatorAdapter
        public Action<DarkLightEvent> OnBroadcast { get; set; }

        private List<BacktestRequestEvent> runQueue;

        public ServiceBusLocal()
        {
            runQueue = new List<BacktestRequestEvent>();
        }

        //IMediatorAdapter, IBacktestAdapter
        public void Publish(DarkLightEvent darkLightEvent)
        {
            switch (darkLightEvent.EventType)
            {
                case (EventType.BacktestRequest):
                    var requestEvent = (BacktestRequestEvent)darkLightEvent;
                    if (requestEvent.ActionType == ServiceAction.Run)
                        OnRunBacktest(requestEvent.HistDataService, requestEvent.Response);
                    else if (requestEvent.ActionType == ServiceAction.Pause)
                        OnPauseBacktest(requestEvent.Key);
                    else if (requestEvent.ActionType == ServiceAction.Resume)
                        OnResumeBacktest(requestEvent.Key);
                    else if (requestEvent.ActionType == ServiceAction.Cancel)
                        OnCancelBacktest(requestEvent.Key);                   
                    break;
                case (EventType.Status):
                    OnBroadcast(darkLightEvent);
                    break;
                case (EventType.Result):
                    int i = 0;
                    break;
                case (EventType.Trade):
                    var tradeEvent = (TradeEvent) darkLightEvent;
                    break;
            }
        }

        public void PublishCEP(object nesperEvent)
        {

        }

        public void SubscribeCEP(EventType eventType, UpdateEventHandler handler)
        {
            
        } 
    }
}
