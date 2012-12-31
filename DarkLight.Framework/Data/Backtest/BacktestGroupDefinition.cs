using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Framework.Data.Backtest
{
    public class BacktestGroupDefinition
    {
        public string ResponseType { get; set; }
        public string OptimizationType { get; set; }
        public ConfigurationSpace ConfigSpace { get; set; }

    }
}
