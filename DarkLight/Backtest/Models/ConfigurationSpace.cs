using System;
using System.Collections.Generic;
using System.Text;


namespace DarkLight.Backtest.Models
{
    public class ConfigurationSpace
    {
        public List<ConfigurationVariableSpace<double>> ParameterSpace;
        public ConfigurationVariableSpace<DateTime> TemporalSpace;
        public List<string> ProductSpace;

        public int GetSpaceSize()
        {
            int numSpacePoints = 1;
            foreach (var paramSpace in ParameterSpace)
            {
                numSpacePoints *= paramSpace.Quantity;
            }
            numSpacePoints *= TemporalSpace.Quantity;

            return numSpacePoints;
        }
    }
}
