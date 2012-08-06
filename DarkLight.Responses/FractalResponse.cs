using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DarkLight.Utilities;
using TradeLink.API;
using TradeLink.Common;
using System.Linq;

namespace DarkLight.Responses
{
    public class FractalResponse : ResponseTemplate
    {
        int _time = 0;
        int[] _intervalValues = new [] { 1, 2, 5, 10, 15, 30, 60};
        BarInterval[] _intervalTypes = new[] {BarInterval.CustomTime, BarInterval.CustomTime, BarInterval.CustomTime, BarInterval.CustomTime, BarInterval.CustomTime, BarInterval.CustomTime, BarInterval.CustomTime };
        BarListTracker _barListTracker = new BarListTracker();
        Dictionary<string, Color> _symbolColorMap = new Dictionary<string, Color>();
        List<string> _symbols = new List<string>{"JPM","WFC"}; 
        //[Description("Time interval for dimensionality measurement.")]
        //public int ObservationIntervalInSeconds { get { return _observationIntervalInSeconds; } set { _observationIntervalInSeconds = value; } }

        //[Description("Number intervals for dimensionality measurement.")]
        //public int NumberIntervalsInWindow { get { return _numberIntervalsInWindow; } set { _numberIntervalsInWindow = value; } }

        //[Description("Calculation interval.")]
        //public int CalculationIntervalInSeconds { get { return _calculationIntervalInSeconds; } set { _calculationIntervalInSeconds = value; } }

        public FractalResponse()
        {
            Setup();
        }

        public override void Reset()
        {
            Setup();
            base.Reset();
        }

        private void Setup()
        {
            _time = 0;

            var _colors = PlottingUtilities.GetLegacyColorList(_symbols.Count);
            for (int i = 0; i < _symbols.Count; i++ )
            {
                _symbolColorMap.Add(_symbols[i],_colors[i]);
            }

            _barListTracker = new BarListTracker();
            _barListTracker = new BarListTracker(_intervalValues, _intervalTypes);
            _barListTracker.GotNewBar += _barListTracker_GotNewBar;
            isValid = true;
        }

        void _barListTracker_GotNewBar(string symbol, int interval)
        {
            switch (interval)
            {
                case 60:
                {
                    var barList1 = _barListTracker[symbol, 1];
                    var barList2 = _barListTracker[symbol, 2];

                    if (barList1.Has(60) && barList2.Has(30))
                    {
                        var closeList1 = Calc.EndSlice(barList1.Close(), 60);
                        var closeList2 = Calc.EndSlice(barList2.Close(), 30);
                        var closeMetric1 = GetFractalMetric(closeList1);
                        var closeMetric2 = GetFractalMetric(closeList2);
                        var dimension = closeMetric1/closeMetric2;
                        sendchartlabel(dimension, _time, "Dimension", _symbolColorMap[symbol]);
                    }
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        decimal GetFractalMetric(decimal[] prices)
        {
            int step = 2;
            decimal metric = 0.0m;
            for(int i = 1; i < prices.Length; i++)
            {
                metric += Math.Abs(prices[i] - prices[i - 1]);
            }
            return metric;
        }



        public override void GotTick(Tick tick)
        {
            if (!isValid) return;
            if (!tick.isTrade) return;
            _time = tick.time;
            _barListTracker.newTick(tick);
        }
    }
}
