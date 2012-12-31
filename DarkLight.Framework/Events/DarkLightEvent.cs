using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Framework.Enums;

namespace DarkLight.Framework.Events
{
    public class DarkLightEvent
    {
        public string Key { get; set; }
        public EventType EventType { get; set; }
    }
}
