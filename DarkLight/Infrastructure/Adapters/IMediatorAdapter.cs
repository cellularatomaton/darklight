using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Events;
using com.espertech.esper.client;
using EventType = DarkLight.Events.EventType;

namespace DarkLight.Infrastructure.Adapters
{
    public interface IMediatorAdapter : IAdapter
    {
        Action<DarkLightEvent> OnBroadcast { get; set; }
        void PublishCEP(object nesperEvent);
        void SubscribeCEP(EventType eventType, UpdateEventHandler handler);
    }
}
