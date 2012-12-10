using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using DarkLight.Events;
using DarkLight.Infrastructure.Adapters;
using com.espertech.esper.client;
using EventType = DarkLight.Events.EventType;

namespace DarkLight.Infrastructure.WPFClient
{
    public class MediatorCEP : IMediator
    {
        #region Private Members
        
        IMediatorAdapter _mediatorAdapter;

        #endregion

        #region Constructors

        public MediatorCEP(IMediatorAdapter mediatorAdapter)
        {
            _mediatorAdapter = mediatorAdapter;           
        }

        #endregion

        #region Implementation of IMediator

        public void Broadcast(DarkLightEvent darkLightEvent)
        {
            _mediatorAdapter.PublishCEP(darkLightEvent);
        }

        public void Subscribe(EventType eventType, UpdateEventHandler handler)
        {
            _mediatorAdapter.SubscribeCEP(eventType, handler);
        }

        #endregion
    }
}
