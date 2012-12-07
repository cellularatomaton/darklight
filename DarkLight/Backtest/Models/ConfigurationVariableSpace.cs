using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Backtest.Models
{
    public class ConfigurationVariableSpace<T>
    {
        public string Name { get; set; }
        public T Min { get; set; }
        public T Max { get; set; }
        public double Step { get; set; }
        public int Quantity { get; set; }

        public ConfigurationVariableSpace(string name, T min, T max, double step)
        {
            Name = name;
            Step = step;
            Min = min;
            Max = max;

            if (typeof(T) == typeof(DateTime))
            {
                DateTime dateMax = Convert.ToDateTime(max);
                DateTime dateMin = Convert.ToDateTime(min);
                var totalQuantity = (int)(dateMax.Subtract(dateMin).TotalDays + 1);
                
                for (int d = 0; d < totalQuantity; d++)
                {
                    DateTime currentDate = dateMin.AddDays(d);
                    //if (currentDate.DayOfWeek != DayOfWeek.Saturday || currentDate.DayOfWeek != DayOfWeek.Sunday)
                        Quantity++;
                }
            }
            else
            {
                double dMax = Convert.ToDouble(max);
                double dMin = Convert.ToDouble(min);
                Quantity = (int)((dMax - dMin) / step + 1);
            }            
        }
    }
}
