using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Adapters;
using DarkLight.Framework.Interfaces.Common;
using com.espertech.esper.client;
using EventType = DarkLight.Framework.Enums.EventType;


namespace DarkLight.Infrastructure.Mediator
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
