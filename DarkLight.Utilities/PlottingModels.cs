using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using TradeLink.AppKit;

namespace DarkLight.Utilities
{
    public class PlottablePoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class PlottableValue<T>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public T Value { get; set; }
    }

    public class TimePlot
    {
        public bool Selected { get; set; }
        public string Label { get; set; }
        public Color PointColor { get; set; }
        public List<TimePlotPoint> PlotPoints { get; set; }
    }

    public class TimePlotPoint
    {
        public DateTime Time { get; set; }
        public decimal Value { get; set; }
    }

    public class PlottableProperty : INotifyPropertyChanged
    {
        private string _propertyName;
        public string PropertyName
        {
            get { return _propertyName; }
            set
            {
                if (value != _propertyName)
                {
                    _propertyName = value;
                    NotifyPropertyChanged("PropertyName");
                }
            }
        }

        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    NotifyPropertyChanged("Selected");
                }
            }
        }

        private Color _plotColor;
        public Color PlotColor
        {
            get { return _plotColor; }
            set
            {
                if (value != _plotColor)
                {
                    _plotColor = value;
                    NotifyPropertyChanged("PlotColor");
                }
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }

    public class DarkLightResults : Results
    {
        public decimal NetProfitOrLoss { get; set; }

        public DarkLightResults(Results results)
        {
            PlottingUtilities.CopyParameters(results, this);
            NetProfitOrLoss = GrossPL - (HundredLots*100*ComPerShare);
        }
    }
}