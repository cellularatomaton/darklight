using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Events;

namespace DarkLight.Infrastructure.Adapters
{
    public interface IAdapter
    {
        void Publish(DarkLightEvent darkLightEvent);
    }
}
