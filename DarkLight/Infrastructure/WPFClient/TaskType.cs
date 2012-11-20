using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Infrastructure
{
    public enum TaskType
    {
        // base
        Background,
        //clock
        Periodic,
        //interrupt
        Sporadic,
        //UI
        LongRunning
    }
}
