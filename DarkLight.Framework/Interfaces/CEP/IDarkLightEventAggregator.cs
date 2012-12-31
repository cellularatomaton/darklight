using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Framework.Events;

namespace DarkLight.Framework.Interfaces.CEP
{
    public interface IDarkLightEventAggregator
    {
        void Publish(DarkLightEvent darkLightEvent);
    }
}
