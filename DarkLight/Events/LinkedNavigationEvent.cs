using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace DarkLight.Events
{
    public class LinkedNavigationEvent
    {
        public string Key { get; set; }
        public LinkGroup LinkGroup { get; set; }
        public Color ColorGroup { get; set; }
        public NavigationDestination Destination { get; set; }
    }
}
