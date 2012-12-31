using System;
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
    public class ServiceBusLocalCEP : IMediatorAdapter, IBacktestAdapter
    {
        //CEP
        EPServiceProvider _epService;
        Dictionary<EventType, EPStatement> statementDict;

        //IBacktestAdapter
        public Action<IHistDataService, DarkLightResponse> OnRunBacktest { get; set; }
        public Action<string> OnPauseBacktest { get; set; }
        public Action<string> OnResumeBacktest { get; set; }
        public Action<string> OnCancelBacktest { get; set; }
        
        //IMediatorAdapter
        public Action<DarkLightEvent> OnBroadcast { get; set; }

        public ServiceBusLocalCEP()
        {            
            Configuration configuration = new Configuration();
            configuration.AddEventType("BacktestRequestEvent", typeof(BacktestRequestEvent).FullName);
            configuration.AddEventType("LinkedNavigationEvent", typeof(LinkedNavigationEvent).FullName);
            configuration.AddEventType("StatusEvent", typeof(StatusEvent).FullName);
            configuration.AddEventType("TradeEvent", typeof(TradeEvent).FullName);
            configuration.EngineDefaults.EventMetaConfig.ClassPropertyResolutionStyle = PropertyResolutionStyle.CASE_INSENSITIVE;
            _epService = EPServiceProviderManager.GetProvider("ServiceBusLocalCEP", configuration);

            statementDict = new Dictionary<EventType, EPStatement>();
            statementDict.Add(EventType.BacktestRequest, _epService.EPAdministrator.CreateEPL("select * from BacktestRequestEvent"));
            statementDict.Add(EventType.LinkedNavigation, _epService.EPAdministrator.CreateEPL("select * from LinkedNavigationEvent"));
            statementDict.Add(EventType.Status, _epService.EPAdministrator.CreateEPL("select * from StatusEvent"));            
            statementDict.Add(EventType.Trade, _epService.EPAdministrator.CreateEPL("select * from TradeEvent"));

            statementDict[EventType.BacktestRequest].Events += HandleBacktestRequest;
        }

        //IMediatorAdapter, IBacktestAdapter
        public void Publish(DarkLightEvent darkLightEvent)
        {
            PublishCEP(darkLightEvent);
        }

        public void PublishCEP(object nesperEvent)
        {
            _epService.EPRuntime.SendEvent(nesperEvent);
        }

        public void SubscribeCEP(EventType eventType, UpdateEventHandler handler)
        {
            statementDict[eventType].Events += handler;
        }    

        public void HandleBacktestRequest(object sender, UpdateEventArgs e)
        {
            var backtestRequestEvent = (BacktestRequestEvent)e.NewEvents[0].Underlying;
            if (backtestRequestEvent.ActionType == ServiceAction.Run)
                OnRunBacktest(backtestRequestEvent.HistDataService, backtestRequestEvent.Response);
            else if (backtestRequestEvent.ActionType == ServiceAction.Pause)
                OnPauseBacktest(backtestRequestEvent.Key);
            else if (backtestRequestEvent.ActionType == ServiceAction.Resume)
                OnResumeBacktest(backtestRequestEvent.Key);
        }
    }
}
