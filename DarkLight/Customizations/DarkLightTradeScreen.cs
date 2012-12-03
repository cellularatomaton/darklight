using Caliburn.Micro;
using DarkLight.Enums;
using DarkLight.Events;
using DarkLight.Services;
using DarkLight.Utilities;

namespace DarkLight.Customizations
{
    public class DarkLightTradeScreen : DarkLightScreen, IHandle<TradeEvent>
    {
        public string Key { get; set; }  //= "teststrategy";
        public TradeMode Mode { get; set; }
        public string ResponseType { get; set; }
        public string Parameters { get; set; }

        public DarkLightTradeScreen()
        {
            IoC.Get<IEventAggregator>().Subscribe(this);
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
            //TODO refactor
            var record = MockUtilities.ParseSessionGUID(linkedNavigationEvent.Key);            
            
            Mode = record.Mode;
            ResponseType = record.ResponseType;
            Parameters = record.Parameters;
        }

    }
}
