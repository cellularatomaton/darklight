using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DarkLight.Utilities;
using OxyPlot;

namespace DarkLight.Analytics
{
    /// <summary>
    /// The complex type statistics model is used for looking at the distributional characteristics of a complex type with several
    /// public properties and fields.  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComplexTypeStatisticsModel<T> : INotifyPropertyChanged
    {
        public ComplexTypeStatisticsModel(IEnumerable<T> sampleInstances)
        {
            var viewableType = typeof (T);
            var viewableProperties = PlottingUtilities.GetAllPlottableValues(viewableType);
            ViewableProperties = new ObservableCollection<PlottableProperty>(viewableProperties);
            _sampleInstances = new ObservableCollection<T>(sampleInstances);
        }

        private ObservableCollection<PlottableProperty> _viewableProperties = new ObservableCollection<PlottableProperty>();
        public ObservableCollection<PlottableProperty> ViewableProperties
        {
            get { return _viewableProperties; }
            set
            {
                if (value != _viewableProperties)
                {
                    _viewableProperties = value;
                    NotifyPropertyChanged("ViewableProperties");
                }
            }
        }

        private PlottableProperty _selectedViewableProperty;
        public PlottableProperty SelectedViewableProperty
        {
            get { return _selectedViewableProperty; }
            set
            {
                if (value != _selectedViewableProperty)
                {
                    _selectedViewableProperty = value;
                    PopulateDescriptiveStatistics(_selectedViewableProperty);
                    NotifyPropertyChanged("SelectedViewableProperty");
                }
            }
        }

        ObservableCollection<KeyValuePair<string, string>> _statisticsList = new ObservableCollection<KeyValuePair<string, string>>();
        public ObservableCollection<KeyValuePair<string, string>> StatisticsList
        {
            get { return _statisticsList; }
            set
            {
                if (value != _statisticsList)
                {
                    _statisticsList = value;
                    NotifyPropertyChanged("StatisticsList");
                }
            }
        }

        private DescriptiveResult _statistics;
        public DescriptiveResult Statistics
        {
            get { return _statistics; }
            set
            {
                if (value != _statistics)
                {
                    _statistics = value;
                    NotifyPropertyChanged("Statistics");
                }
            }
        }

        private int _numberBins = 25;
        public int NumberBins
        {
            get { return _numberBins; }
            set
            {
                if (value != _numberBins)
                {
                    _numberBins = value;
                    NotifyPropertyChanged("NumberBins");
                }
            }
        }

        private PlotModel _histogramModel;
        public PlotModel HistogramModel
        {
            get { return _histogramModel; }
            set
            {
                if (value != _histogramModel)
                {
                    _histogramModel = value;
                    NotifyPropertyChanged("HistogramModel");
                }
            }
        }

        private ObservableCollection<T> _sampleInstances = new ObservableCollection<T>();
        public ObservableCollection<T> SampleInstances
        {
            get { return _sampleInstances; }
            set
            {
                if (value != _sampleInstances)
                {
                    _sampleInstances = value;
                    NotifyPropertyChanged("SampleInstances");
                }
            }
        }

        public void PopulateDescriptiveStatistics(PlottableProperty plottableProperty)
        {
            var plottableValues = SampleInstances.Select(i => new PlottableValue<T> {X=SampleInstances.IndexOf(i), Value = i}).ToList();
            var plottablePoints = PlottingUtilities.ToPlottable<T>(plottableValues, plottableProperty).Select(p => p.Y).ToArray();
            if(plottablePoints.Count() != 0)
            {
                PopulateBins(plottablePoints, _numberBins, plottableProperty.PropertyName);
                Descriptive descriptiveStats = new Descriptive(plottablePoints);
                descriptiveStats.Analyze();
                Statistics = descriptiveStats.Result;
                StatisticsList = new ObservableCollection<KeyValuePair<string, string>>(PlottingUtilities.GetFieldAndPropertyValueList(descriptiveStats.Result));
            }
        }

        public void PopulateBins(IEnumerable<double> samples, int numBins, string title)
        {
            var max = samples.Max();
            var min = samples.Min();
            var step = (max - min)/Convert.ToDouble(numBins);
            var binRange = Range.Double(min, max, step);

            var model = new PlotModel(title) {};
            var s1 = new RectangleBarSeries {FillColor = OxyColors.CornflowerBlue};

            foreach (var binMin in binRange)
            {
                var binMax = binMin + step;
                var binSamples = samples.Where(v => binMin <= v && v < binMax);
                var binCount = Convert.ToDouble(binSamples.Count());
                s1.Items.Add(new RectangleBarItem
                {
                    X0 = binMin, 
                    X1 = binMax, 
                    Y0 = 0, 
                    Y1 = binCount,
                });
            }
            model.Series.Add(s1);
            model.PlotMargins = new OxyThickness(0, 30, 0, 0);
            var xMin = s1.Items.Min(b => b.X0);
            var xMax = s1.Items.Max(b => b.X1);
            if(xMin < xMax)
            {
                var xAxis = new LinearAxis(AxisPosition.Bottom, xMin, xMax, "Value");
                xAxis.AbsoluteMinimum = xMin;
                xAxis.AbsoluteMaximum = xMax;
                xAxis.SetColors();
                model.Axes.Add(xAxis);
            }

            var yMin = s1.Items.Min(b => b.Y0);
            var yMax = s1.Items.Max(b => b.Y1);
            if(yMin < yMax)
            {
                var yAxis = new LinearAxis(AxisPosition.Left, s1.Items.Min(b => b.Y0), s1.Items.Max(b => b.Y1), "Frequency");
                yAxis.AbsoluteMinimum = s1.Items.Min(b => b.Y0);
                yAxis.AbsoluteMaximum = s1.Items.Max(b => b.Y1);
                yAxis.SetColors();
                model.Axes.Add(yAxis);
            }
            model.SetColors();
            HistogramModel = model;
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
}