using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Framework.Events;

namespace DarkLight.Framework.Interfaces.Adapters
{
    public interface IAdapter
    {
        void Publish(DarkLightEvent darkLightEvent);
    }
}
