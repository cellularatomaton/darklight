using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Events
{
    public class ServiceEventBase
    {
        public ServiceType ServiceType { get; set; }
        public string Key { get; set; }
    }
}
