using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Backtest.Models;

namespace DarkLight.Utilities
{
    public static class CommonFunctions
    {
        public static string GenerateBacktestGroupGUID(string responseType, ConfigurationSpace configSpace)
        {
            string GUID = responseType + "|";
            foreach (var param in configSpace.ParameterSpace)
            {
                GUID += param.Name + "=[" + param.Min + "," + param.Max + "]|";
            }
            GUID += configSpace.TemporalSpace.Min.ToString("MM/dd/yyy") + "|" + configSpace.TemporalSpace.Max.ToString("MM/dd/yyy");
            return GUID;
        }
    }
}
