using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Framework.Events;
using com.espertech.esper.client;
using EventType = DarkLight.Framework.Enums.EventType;

namespace DarkLight.Framework.Interfaces.Adapters
{
    public interface IMediatorAdapter : IAdapter
    {
        Action<DarkLightEvent> OnBroadcast { get; set; }
        void PublishCEP(object nesperEvent);
        void SubscribeCEP(EventType eventType, UpdateEventHandler handler);
    }
}
