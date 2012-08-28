using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DarkLight.Utilities;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace DarkLight.Analytics
{
    /// <summary>
    /// Interaction logic for StatisticsControl.xaml
    /// </summary>
    public partial class StatisticsControl : UserControl
    {
        public StatisticsControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //HistogramPlotter.AxisGrid.DrawHorizontalMinorTicks = false;
            //HistogramPlotter.AxisGrid.DrawHorizontalTicks = false;
            //HistogramPlotter.AxisGrid.DrawVerticalMinorTicks = false;
            //HistogramPlotter.AxisGrid.DrawVerticalTicks = false;
        }

        public void SetHistogramPlotterDomain(DescriptiveResult stats)
        {
            //HistogramPlotter.Viewport.Domain = new DataRect(stats.Min, 0, stats.Range, stats.Count);
            //barChart.Plotter.Viewport.Domain = new DataRect(stats.Min, 0, stats.Range, stats.Count);
        }
    }

    public class BarChartData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
    }

    public class StatisticsModel<T> : INotifyPropertyChanged
    {
        public StatisticsModel(IEnumerable<T> sampleInstances)
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

        private ObservableCollection<BarChartData> _bins = new ObservableCollection<BarChartData>();
        public ObservableCollection<BarChartData> Bins
        {
            get
            {
                return _bins;
            }
            set
            {
                if (value != _bins)
                {
                    _bins = value;
                    NotifyPropertyChanged("Bins");
                }
            }
        }

        EnumerableDataSource<BarChartData> _binDataSource;
        public EnumerableDataSource<BarChartData> BinDataSource
        {
            get
            {
                return _binDataSource;
            }
            set
            {
                value.SetXMapping(bar => bar.X);
                value.SetYMapping(bar => bar.Height);
                _binDataSource = value;
            }
        }

        public BarChartData[] BinArray { get; set; }

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
                PopulateBins(plottablePoints, _numberBins);
                Descriptive descriptiveStats = new Descriptive(plottablePoints);
                descriptiveStats.Analyze();
                Statistics = descriptiveStats.Result;
                StatisticsList = new ObservableCollection<KeyValuePair<string, string>>(PlottingUtilities.GetFieldAndPropertyValueList(descriptiveStats.Result));
            }
        }

        public void PopulateBins(IEnumerable<double> samples, int numBins)
        {
            //Bins.Clear();
            //BarSegments.Clear();
            var bins = new List<BarChartData>();
            var max = samples.Max();
            var min = samples.Min();
            var step = (max - min)/Convert.ToDouble(numBins);
            var binRange = Range.Double(min, max, step);
            var yMax = 0.0;
            foreach (var binMin in binRange)
            {
                var binMax = binMin + step;
                var binSamples = samples.Where(v => binMin < v && v < binMax);
                var binVal = (binMin + binMax)/2.0;
                var binCount = Convert.ToDouble(binSamples.Count());
                yMax = yMax < binCount ? binCount : yMax;
                var bar = new BarChartData { X = binVal, Y = 0.0, Height = binCount, Width = step };
                bins.Add(bar);
                //var segment = new Segment {};
            }
            Bins = new ObservableCollection<BarChartData>(bins);
            //BinDataSource = new EnumerableDataSource<BarChartData>(bins);
            //BinArray = bins.ToArray();
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
