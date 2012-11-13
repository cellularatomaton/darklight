using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace DarkLight.Events
{
    public enum TradeEventType
    {
        Fill,
        Order,
        Message,
        Tick
    }
}
