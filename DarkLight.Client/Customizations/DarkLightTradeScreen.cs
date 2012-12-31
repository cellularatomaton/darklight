using System;
using Caliburn.Micro;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Common;
using DarkLight.Framework.Interfaces.Services;
using DarkLight.Framework.Utilities;
using DarkLight.Infrastructure.Mediator;
using com.espertech.esper.client;
using EventType = DarkLight.Framework.Enums.EventType;

namespace DarkLight.Client.Customizations
{
    public class DarkLightTradeScreen : DarkLightScreen, IHandle<TradeEvent>
    {
        public string Key { get; set; }  
        //public TradeMode Mode { get; set; }
        public string ResponseType { get; set; }
        public string Parameters { get; set; }
        public string TradeDate { get; set; }

        public DarkLightTradeScreen()
        {
            var mediator = IoC.Get<IMediator>();
            if (mediator.GetType() == typeof(Mediator))
                IoC.Get<IEventAggregator>().Subscribe(this);
            else if (mediator.GetType() == typeof(MediatorCEP))
                mediator.Subscribe(EventType.Trade, UpdateFromCEP);
        }

        protected virtual void AddTrade(TradeEvent tradeEvent)
        {            
        }

        public virtual void Handle(TradeEvent tradeEvent)
        {
            var _filter = IoC.Get<IFilterService>().GetTradeFilter(Key);
            if (_filter.IsPassedBy(tradeEvent))
            {
                AddTrade(tradeEvent);
            }
        }

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {                       
            Key = linkedNavigationEvent.Key;
            ResponseType = MockUtilities.GetResponseFromGUID(Key);
            Parameters = MockUtilities.GetParametersFromGUID(Key);
            TradeDate = MockUtilities.GetTradeDateFromGUID(Key);
        }

        public void UpdateFromCEP(object sender, UpdateEventArgs e)
        {
            var tradeEvent = (TradeEvent)e.NewEvents[0].Underlying;
            Handle(tradeEvent);
        }
    }
}
