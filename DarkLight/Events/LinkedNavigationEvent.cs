using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace DarkLight.Events
{
    public class LinkedNavigationEvent : DarkLightEvent
    {
        public NavigationAction NavigationAction { get; set; }
        public Color ColorGroup { get; set; }
        public NavigationDestination Destination { get; set; }
        public NavigationGroup Group { get; set; }
        public string Message { get; set; }

        public LinkedNavigationEvent()
        {
            EventType = EventType.LinkedNavigation;
        }
    }
}
