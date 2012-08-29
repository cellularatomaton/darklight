
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using DarkLight.Analytics.Models;
using DarkLight.Utilities;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Markers2;
using Microsoft.Win32;
using TradeLink.Common;


namespace DarkLight.Analytics
{
    /// <summary>
    /// Interaction logic for BacktestingControl.xaml
    /// </summary>
    /// 

    public partial class BacktestingControl : UserControl
    {
        // Shared Models:
        static ActivityModel _activityModel = new ActivityModel();
        static Backtest _backtest = new Backtest();
        static BacktestingConfigurationModel _backtestingConfigurationModel = new BacktestingConfigurationModel();
        static ResponseLibraryList _backtestingResponseLibraryList = new ResponseLibraryList();

        bool _initializationModelsUnbound = false;
        bool _reportModelsUnbound = false;
        bool _responseModelsUnbound = false;

        public BacktestingControl()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += LoadFromResponseFolder;

            InitializeComponent();
        }

        // This method adds our selected reponse directory to the assembly search path.
        private Assembly LoadFromResponseFolder(object sender, ResolveEventArgs args)
        {
            if(!string.IsNullOrEmpty(_backtestingResponseLibraryList.FileName))
            {
                string folderPath = System.IO.Path.GetDirectoryName(_backtestingResponseLibraryList.FileName);
                string assemblyPath = System.IO.Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
                if (File.Exists(assemblyPath) == false) return null;
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                return assembly;
            }
            else
            {
                return Assembly.GetExecutingAssembly();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UnbindInitializationModels();

            _backtestingResponseLibraryList.LoadResponseListFromFileName(Properties.Settings.Default.ResponseFileName);

            BacktestPlotter.AxisGrid.DrawHorizontalMinorTicks = false;
            BacktestPlotter.AxisGrid.DrawHorizontalTicks = false;
            BacktestPlotter.AxisGrid.DrawVerticalMinorTicks = false;
            BacktestPlotter.AxisGrid.DrawVerticalTicks = false;

            _backtest = new Backtest();
            _backtest.PropertyChanged += (o, args) =>
            {
                if (args.PropertyName == "SelectedReport")
                {
                    UpdateBacktestPlots();
                    BindReportModels();
                }
            };

            BindInitializationModels();
        }

        #region Backtesting GUI Event Handlers

        private void BacktestingResponseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Responses Libraries|*.dll|AllFiles|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _backtestingResponseLibraryList.LoadResponseListFromFileName(openFileDialog.FileName);
            }
        }

        private void BacktestingComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_initializationModelsUnbound ||
                _reportModelsUnbound ||
                _responseModelsUnbound)
            {
                return;
            }
            UnbindResponseModels();
            _backtestingConfigurationModel.ResponseName = _backtestingResponseLibraryList.SelectedResponse;
            _backtestingConfigurationModel.ResponsePath = _backtestingResponseLibraryList.FileName;
            if (_backtestingConfigurationModel.ResponseName != ResponseLibraryList._header)
            {
                _backtestingConfigurationModel.SelectedResponse = ResponseLoader.FromDLL(_backtestingConfigurationModel.ResponseName, _backtestingConfigurationModel.ResponsePath);
            }
            BindResponseModels();
        }

        private void RunBacktestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UnbindInitializationModels();
            UnbindReportModels();

            _backtest.Clear();
            _activityModel.Status = "Backtest started.";
            var tickDataGroups = BacktestingTickFileControl.GetSelectedFilePaths();

            _activityModel.NumberTestsToRun = tickDataGroups.Count;
            _activityModel.NumberTestsCompleted = 0;

            foreach (var _tickDataGroup in tickDataGroups)
            {
                var _backtestingModel = new BacktestingModel();
                _backtestingModel.CacheWait = _backtestingConfigurationModel.CacheWait;
                var _reportModel = new BatchReportModel(_backtestingModel, _activityModel /*, updatePlots*/);
                var info = TickFileNameInfo.GetTickFileInfoFromLongName(_tickDataGroup.First());
                _reportModel.ReportName = "Y:" + info.Year.ToString() + ",M:" + info.Month.ToString() + ",D:" + info.Day.ToString();
                
                _backtest.AddRun(_backtestingModel,_reportModel);

                _reportModel.RegisterDispatcher(DispatchableType.Fill, FillsTab.Dispatcher);
                _reportModel.RegisterDispatcher(DispatchableType.Indicator, IndicatorTab.Dispatcher);
                _reportModel.RegisterDispatcher(DispatchableType.Message, MessagesTab.Dispatcher);
                _reportModel.RegisterDispatcher(DispatchableType.Order, OrdersTab.Dispatcher);
                _reportModel.RegisterDispatcher(DispatchableType.Plot, BacktestPlotter.Dispatcher);
                _reportModel.RegisterDispatcher(DispatchableType.Position, PositionTab.Dispatcher);
            
                BindInitializationModels();
            
                _backtestingModel.LoadResponse(_backtestingConfigurationModel.GetFreshResponseInstance());
                _backtestingModel.LoadTickData(_tickDataGroup);
                _backtestingModel.Play(_backtestingConfigurationModel.SelectedPlayToValue);
            }

            Task.Factory.StartNew(() =>
            {
                _activityModel.AllRunsCompleted.WaitOne();
                if (_backtest.BacktestReports.Any())
                {
                    _backtest.SelectedReport = _backtest.BacktestReports.First();
                    BindStatisticsModels();
                }
            });
        }

        private void BacktestingPlotUpdateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateBacktestPlots();
        }

        #endregion

        private void BindInitializationModels()
        {
            ResultsSelectionListBox.DataContext = _backtest;
            BacktestingResponseExpander.DataContext = _backtestingResponseLibraryList;
            BacktestingRunExpander.DataContext = _backtestingConfigurationModel;
            BacktestingStatusBar.DataContext = _activityModel;
            _initializationModelsUnbound = false;
        }

        private void UnbindInitializationModels()
        {
            _initializationModelsUnbound = true;
            BacktestingResponseExpander.DataContext = null;
            BacktestingTickDataExpander.DataContext = null;
            BacktestingRunExpander.DataContext = null;
            BacktestingStatusBar.DataContext = null;
        }

        private void BindResponseModels()
        {
            BacktestingResponsePropertyGrid.DataContext = _backtestingConfigurationModel.SelectedResponse;
            _responseModelsUnbound = false;
        }

        private void UnbindResponseModels()
        {
            _responseModelsUnbound = true;
            BacktestingResponsePropertyGrid.DataContext = null;
        }

        private void BindReportModels()
        {
            BacktestingTabControl.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                MessagesDataGrid.DataContext = _backtest.SelectedReport;
                BacktestingTabControl.DataContext = _backtest.SelectedReport;
                BacktestingPlotExpander.DataContext = _backtest.SelectedReport;

                

                _reportModelsUnbound = false;
            }));
        }

        private void UnbindReportModels()
        {
            _reportModelsUnbound = true;
            MessagesDataGrid.DataContext = null;
            BacktestingTabControl.DataContext = null;
            BacktestingPlotExpander.DataContext = null;
        }

        private void BindStatisticsModels()
        {
            BacktestingStatisticsControl.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            { 
                var results = _backtest.BacktestReports.Select(r => r.Results).ToList();
                var statsModel = new StatisticsModel<DarkLightResults>(results);
                statsModel.SelectedViewableProperty = statsModel.ViewableProperties.First();
                statsModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "Statistics")
                    {
                        var model = sender as StatisticsModel<DarkLightResults>;
                        if (model != null)
                        {
                            BacktestingStatisticsControl.SetHistogramPlotterDomain(model.Statistics);
                        }
                    }
                };
                BacktestingStatisticsControl.DataContext = statsModel;
            }));
        }

        #region Private Backtesting Methods

        private List<IPlotterElement> _charts = new List<IPlotterElement>();
        private void UpdateBacktestPlots()
        {
            BacktestPlotter.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (_backtest.SelectedReport != null)
                {
                    if (_backtest.SelectedReport.Plots.Count != 0)
                    {
                        BacktestPlotter.Children.RemoveAll(_charts.ToArray());
                        _charts.Clear();

                        double maxTime = double.MinValue;
                        double minTime = double.MaxValue;
                        double maxValue = double.MinValue;
                        double minValue = double.MaxValue;

                        foreach (var plot in _backtest.SelectedReport.Plots.Where(p => p.Selected))
                        {
                            var cleanedData =
                                plot.PlotPoints.GroupBy(point => point.Time).Select(group => group.First()).OrderBy(
                                    point => point.Time).ToList();

                            // Viewport info:
                            maxTime = Math.Max(dateAxis.ConvertToDouble(cleanedData.Max(point => point.Time)), maxTime);
                            minTime = Math.Min(dateAxis.ConvertToDouble(cleanedData.Min(point => point.Time)), minTime);
                            maxValue = Math.Max(Convert.ToDouble(cleanedData.Max(point => point.Value)), maxValue);
                            minValue = Math.Min(Convert.ToDouble(cleanedData.Min(point => point.Value)), minValue);

                            var dataSource = CreateBacktestPlotDataSource(cleanedData);
                            LineChart chart = new LineChart
                            {
                                ItemsSource = dataSource,
                                StrokeThickness = 2,
                                Stroke = new SolidColorBrush(plot.PointColor),
                                Description = plot.Label,
                            };
                            _charts.Add(chart);
                            BacktestPlotter.Children.Add(chart);
                        }

                        var viewWidth = maxTime - minTime;
                        var viewHeight = maxValue - minValue;
                        BacktestPlotter.Viewport.Domain = new DataRect(minTime, minValue, viewWidth, viewHeight);
                    }
                    else
                    {
                        _activityModel.Status = "No data to plot.";
                    }
                }
            }));
        }

        private ObservableCollection<Point> CreateBacktestPlotDataSource(IEnumerable<TimePlotPoint> plots)
        {
            var points = plots.Select(p => new Point(dateAxis.ConvertToDouble(p.Time), Convert.ToDouble(p.Value)));
            return new ObservableCollection<Point>(points);
        }

        #endregion

        private void StopBacktestButton_Click(object sender, RoutedEventArgs e)
        {
            _backtest.Stop();
            _backtest.Clear();
        }
    }

    public class Backtest : INotifyPropertyChanged, IDisposable
    {
        BatchReportModel _selectedReport;
        public BatchReportModel SelectedReport
        {
            get { return _selectedReport; }
            set
            {
                if (value != _selectedReport)
                {
                    _selectedReport = value;
                    NotifyPropertyChanged("SelectedReport");
                }
            }
        }

        private ObservableCollection<BatchReportModel> _backtestReports = new ObservableCollection<BatchReportModel>();
        public ObservableCollection<BatchReportModel> BacktestReports
        {
            get { return _backtestReports; }
        }

        private ObservableCollection<BacktestingModel> _backtestModels = new ObservableCollection<BacktestingModel>();
        public ObservableCollection<BacktestingModel> BacktestModels
        {
            get { return _backtestModels; }
        }

        public void AddRun(BacktestingModel testModel, BatchReportModel reportModel)
        {
            _backtestModels.Add(testModel);
            _backtestReports.Add(reportModel);
        }

        public void Stop()
        {
            foreach (var _backtestingModel in BacktestModels)
            {
                _backtestingModel.Stop();
            }
        }

        public void Clear()
        {
            foreach (var _batchReportModel in BacktestReports)
            {
                _batchReportModel.Dispose();
            }
            BacktestReports.Clear();
            foreach (var _backtestingModel in BacktestModels)
            {
                _backtestingModel.Dispose();
            }
            BacktestModels.Clear();
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Clear();
                }
                disposed = true;
            }
        }

        ~Backtest()
        {
            Dispose(false);
        }

        #endregion

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
