﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DarkLight.Analytics.Models;
using DarkLight.Utilities;
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
        static Backtest2 _backtest2 = new Backtest2();
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

        public AnalyticsStatusBar AnalyticsStatusBar
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public TickFileControl TickFileControl
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public StatisticsControl StatisticsControl
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
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
        
        //TOGGLE FOR TEMPORARY TESTING
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UnbindInitializationModels();

            _backtestingResponseLibraryList.LoadResponseListFromFileName(Properties.Settings.Default.ResponseFileName);
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

        private void UserControl_Loaded2(object sender, RoutedEventArgs e)
        {
            UnbindInitializationModels();

            _backtestingResponseLibraryList.LoadResponseListFromFileName(Properties.Settings.Default.ResponseFileName);
            _backtest2 = new Backtest2();
            _backtest2.PropertyChanged += (o, args) =>
            {
                if (args.PropertyName == "SelectedReport")
                {
                    UpdateBacktestPlots2();
                    BindReportModels2();
                }
            };

            BindInitializationModels2();
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
            if (_backtestingConfigurationModel.ResponseName != ResponseLibraryList._header &&
                _backtestingConfigurationModel.ResponseName != null)
            {
                _backtestingConfigurationModel.SelectedResponse = ResponseLoader.FromDLL(_backtestingConfigurationModel.ResponseName, _backtestingConfigurationModel.ResponsePath);
            }
            BindResponseModels();
        }
        
        //TOGGLE FOR TEMPORARY TESTING
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
                    BindStatisticsModels("Results");
                }
            });
        }

        private void RunBacktestButton_Click2(object sender, System.Windows.RoutedEventArgs e)
        {
            UnbindInitializationModels();
            UnbindReportModels();

            _backtest2.Clear();
            _activityModel.Status = "Backtest started.";
            var tickDataGroups = BacktestingTickFileControl.GetSelectedFilePaths();

            _activityModel.NumberTestsToRun = tickDataGroups.Count;
            _activityModel.NumberTestsCompleted = 0;

            Task.Factory.StartNew(() =>
            {
                _activityModel.AllRunsCompleted.WaitOne();
                if (_backtest2.BacktestReports.Any())
                {
                    _backtest2.SelectedReport = _backtest2.BacktestReports.First();
                    BindStatisticsModels2("Results");
                }
            });

            foreach (var _tickDataGroup in tickDataGroups)
            {
                var hubConfig = new HubConfigurationModel(HubType.Local);

                var brokerConfig = new BrokerConfigurationModel(BrokerType.Sim);
                brokerConfig.PlayToValue = _backtestingConfigurationModel.SelectedPlayToValue;
                brokerConfig.SimUseBidAskFills = false;
                brokerConfig.TickFiles = _tickDataGroup;
                brokerConfig.SubscriptionList = new List<byte[]>{EventType.Basket, EventType.CancelOrder, EventType.Order};
                
                var responseConfig = new ResponseConfigurationModel();
                responseConfig.ResponseList.Add(_backtestingConfigurationModel.GetFreshResponseInstance());
                responseConfig.SubscriptionList = new List<byte[]> { EventType.CancelOrderAck, EventType.Fill, EventType.Message, EventType.OrderAck, EventType.Tick };

                var reportConfig = new ReportConfigurationModel();
                reportConfig.Type = ReportType.Batch;
                reportConfig.ActivityInstance = _activityModel;
                var info = TickFileNameInfo.GetTickFileInfoFromLongName(_tickDataGroup.First());
                reportConfig.ReportName = "Y:" + info.Year.ToString() + ",M:" + info.Month.ToString() + ",D:" + info.Day.ToString();
                reportConfig.SubscriptionList = new List<byte[]> { EventType.ChartLabel, EventType.Fill, EventType.Indicator, EventType.Message, EventType.Order, EventType.ServiceTransition, EventType.Tick };
                reportConfig.FilterMode = true;
                
                var sessionModel = new SessionModel(hubConfig, brokerConfig, responseConfig, reportConfig);

                _backtest2.AddRun(sessionModel);

                sessionModel.Reporter.RegisterDispatcher(EventType.Fill, FillsTab.Dispatcher);
                sessionModel.Reporter.RegisterDispatcher(EventType.Indicator, IndicatorTab.Dispatcher);
                sessionModel.Reporter.RegisterDispatcher(EventType.Message, MessagesTab.Dispatcher);
                sessionModel.Reporter.RegisterDispatcher(EventType.Order, OrdersTab.Dispatcher);
                sessionModel.Reporter.RegisterDispatcher(EventType.Plot, BacktestPlotter.Dispatcher);
                sessionModel.Reporter.RegisterDispatcher(EventType.Position, PositionTab.Dispatcher);
            }

            BindInitializationModels2();            
            _backtest2.Start(0);
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

        private void BindInitializationModels2()
        {
            ResultsSelectionListBox.DataContext = _backtest2;
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
                BacktestPlotter.DataContext = _backtest.SelectedReport;
                
                _reportModelsUnbound = false;
            }));
        }

        private void BindReportModels2()
        {
            BacktestingTabControl.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                MessagesDataGrid.DataContext = _backtest2.SelectedReport;
                BacktestingTabControl.DataContext = _backtest2.SelectedReport;
                BacktestingPlotExpander.DataContext = _backtest2.SelectedReport;
                BacktestPlotter.DataContext = _backtest2.SelectedReport;

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

        private void BindStatisticsModels(string statisticsTarget)
        {
            BacktestingStatisticsControl.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                if(statisticsTarget == "Plots") // Show statistics for any plotted points.
                {
                    var plotCollections = _backtest.GetPlotCollectionsByLabel();
                    if (plotCollections.Count != 0)
                    {
                        var statsModel = new PrimativeTypeStatisticsModel(plotCollections);
                        statsModel.SelectedViewableProperty = statsModel.ViewableProperties.First();
                        BacktestingStatisticsControl.DataContext = statsModel;
                    }
                }
                else // Show statistics for results (default behavior)
                {
                    var results = _backtest.BacktestReports.Select(r => r.Results).ToList();
                    var statsModel = new ComplexTypeStatisticsModel<DarkLightResults>(results);
                    statsModel.SelectedViewableProperty = statsModel.ViewableProperties.First();
                    BacktestingStatisticsControl.DataContext = statsModel;
                }
                
            }));
        }

        private void BindStatisticsModels2(string statisticsTarget)
        {
            BacktestingStatisticsControl.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (statisticsTarget == "Plots") // Show statistics for any plotted points.
                {
                    var plotCollections = _backtest2.GetPlotCollectionsByLabel();
                    if (plotCollections.Count != 0)
                    {
                        var statsModel = new PrimativeTypeStatisticsModel(plotCollections);
                        statsModel.SelectedViewableProperty = statsModel.ViewableProperties.First();
                        //statsModel.PropertyChanged += (sender, args) =>
                        //{
                        //    if (args.PropertyName == "Statistics")
                        //    {
                        //        var model = sender as PrimativeTypeStatisticsModel;
                        //        if (model != null)
                        //        {
                        //            BacktestingStatisticsControl.SetHistogramPlotterDomain(model.Statistics);
                        //        }
                        //    }
                        //};
                        BacktestingStatisticsControl.DataContext = statsModel;
                    }
                }
                else // Show statistics for results (default behavior)
                {
                    var results = _backtest2.BacktestReports.Select(r => r.Results).ToList();
                    var statsModel = new ComplexTypeStatisticsModel<DarkLightResults>(results);
                    statsModel.SelectedViewableProperty = statsModel.ViewableProperties.First();
                    //statsModel.PropertyChanged += (sender, args) =>
                    //{
                    //    if (args.PropertyName == "Statistics")
                    //    {
                    //        var model = sender as ComplexTypeStatisticsModel<DarkLightResults>;
                    //        if (model != null)
                    //        {
                    //            BacktestingStatisticsControl.SetHistogramPlotterDomain(model.Statistics);
                    //        }
                    //    }
                    //};
                    BacktestingStatisticsControl.DataContext = statsModel;
                }

            }));
        }

        #region Private Backtesting Methods

        private void UpdateBacktestPlots()
        {
            BacktestPlotter.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (_backtest.SelectedReport != null)
                {
                    if (_backtest.SelectedReport.Plots.Count != 0)
                    {
                        _backtest.SelectedReport.UpdateReportPlots();
                    }
                    else
                    {
                        _activityModel.Status = "No data to plot.";
                    }
                }
            }));
        }

        private void UpdateBacktestPlots2()
        {
            BacktestPlotter.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (_backtest2.SelectedReport != null)
                {
                    if (_backtest2.SelectedReport.Plots.Count != 0)
                    {
                        _backtest2.SelectedReport.UpdateReportPlots();
                    }
                    else
                    {
                        _activityModel.Status = "No data to plot.";
                    }
                }
            }));
        }

        #endregion

        private void StopBacktestButton_Click(object sender, RoutedEventArgs e)
        {
            _backtest.Stop();
            _backtest.Clear();
        }

        private void StatisticsTartetRadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton li = (sender as RadioButton);
            BindStatisticsModels(li.Content.ToString());
        }
    
    }
    

    public class Backtest2 : INotifyPropertyChanged, IDisposable
    {
        #region Private Members

        int _currentSession;
        DateTime _startTime;

        #endregion

        #region Public Members

        IReporter _selectedReport;
        public IReporter SelectedReport
        {
            get { return _selectedReport; }
            set
            {
                if (value != _selectedReport)
                {
                    CopyPlotSelection(_selectedReport, value);
                    _selectedReport = value;
                    NotifyPropertyChanged("SelectedReport");
                }
            }
        }

        private ObservableCollection<IReporter> _backtestReports = new ObservableCollection<IReporter>();
        public ObservableCollection<IReporter> BacktestReports
        {
            get { return _backtestReports; }
        }

        private ObservableCollection<SessionModel> _sessionModels = new ObservableCollection<SessionModel>();
        public ObservableCollection<SessionModel> SessionModels
        {
            get { return _sessionModels; }
        }

        #endregion

        #region Public Methods

        public void AddRun(SessionModel testModel)
        {
            _sessionModels.Add(testModel);
            _backtestReports.Add(testModel.Reporter);
        }

        public void Start(int iSession)
        {
            if (iSession == 0) { _startTime = DateTime.Now;}
            
            _currentSession = iSession;
            SessionModels[_currentSession].Subscribe(EventType.SessionEnd, onSessionEnd);
            SessionModels[_currentSession].Start();
        }

        public void Stop()
        {
            //TODO: implement
            /*
            foreach (var _backtestingModel in BacktestModels)
            {
                _backtestingModel.Stop();
            }
            */
        }

        public void Clear()
        {
            //TODO: implement
            /*
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
            */ 
        }

        public static void CopyPlotSelection(IReporter oldReport, IReporter newReport)
        {
            if (oldReport != null && newReport != null)
            {
                foreach (var _plot in oldReport.Plots)
                {
                    var matchingPlots = newReport.Plots.Where(p => p.Label == _plot.Label);
                    foreach (var _matchingPlot in matchingPlots)
                    {
                        _matchingPlot.Selected = _plot.Selected;
                    }
                }
            }

        }

        //public List<PlottableProperty> GetAllPlottableValues()
        //{
        //    var plottableValueList = BacktestReports.First()
        //        .Plots
        //        .Select(_plot => new PlottableProperty
        //            {
        //                PropertyName = _plot.Label, 
        //                Selected = false,
        //            })
        //        .ToList();
        //    return plottableValueList;
        //}

        public IDictionary<string, List<decimal>> GetPlotCollectionsByLabel()
        {
            Dictionary<string, List<decimal>> plotCollections = new Dictionary<string, List<decimal>>();
            foreach (var _report in BacktestReports)
            {
                foreach (var _plot in _report.Plots)
                {
                    if (!plotCollections.ContainsKey(_plot.Label))
                    {
                        plotCollections[_plot.Label] = new List<decimal>();
                    }
                    foreach (var _point in _plot.PlotPoints)
                    {
                        plotCollections[_plot.Label].Add(_point.Value);
                    }
                }
            }
            return plotCollections;
        }

        #endregion

        #region Private Methods

        void onSessionEnd(DarkLightEventArgs de)
        {
            SessionModels[_currentSession].Unsubscribe(EventType.SessionEnd, onSessionEnd);

            if (_currentSession < SessionModels.Count - 1)
            {
                _currentSession++;
                SessionModels[_currentSession].Subscribe(EventType.SessionEnd, onSessionEnd);
                SessionModels[_currentSession].Start();                
            }
            else
            {
                TimeSpan duration = DateTime.Now -  _startTime;
                var seconds = duration.TotalSeconds;
            }
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
        
        ~Backtest2()
        {
            Dispose(false);
        }

        #endregion       
    }
}