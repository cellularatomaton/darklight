using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Events;

namespace DarkLight.Infrastructure
{
    public interface IMediator
    {
        void Publish(StatusEvent statusEvent);
    }
}
