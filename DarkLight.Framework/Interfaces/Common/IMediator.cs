using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Framework.Events;
using com.espertech.esper.client;
using EventType = DarkLight.Framework.Enums.EventType;

namespace DarkLight.Framework.Interfaces.Common
{
    public interface IMediator
    {
        void Broadcast(DarkLightEvent darkLightEvent);
        void Subscribe(EventType eventType, UpdateEventHandler handler);
    }
}
