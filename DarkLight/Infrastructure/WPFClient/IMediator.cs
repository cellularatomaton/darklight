using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Events;
using com.espertech.esper.client;
using EventType = DarkLight.Events.EventType;

namespace DarkLight.Infrastructure
{
    public interface IMediator
    {
        void Broadcast(DarkLightEvent darkLightEvent);
        void Subscribe(EventType eventType, UpdateEventHandler handler);
    }
}
