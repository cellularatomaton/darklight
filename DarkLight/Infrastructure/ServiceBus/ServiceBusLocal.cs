using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.Infrastructure.Adapters;
using DarkLight.Services;

namespace DarkLight.Infrastructure.ServiceHubs
{
    public class ServiceBusLocal : IMediatorAdapter, IBacktestAdapter
    {        
        //IBacktestAdapter
        public Action<IHistDataService, DarkLightResponse> OnRunBacktest { get; set; }
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
                    OnRunBacktest(requestEvent.HistDataService, requestEvent.Response);                   
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
    }
}
