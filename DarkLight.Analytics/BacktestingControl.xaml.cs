
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DarkLight.Analytics.Models;
using DarkLight.Utilities;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
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

        // Backtesting Models:
        static BatchReportModel _reportModel;
        static BacktestingModel _backtestingModel;
        static BacktestingConfigurationModel _backtestingConfigurationModel = new BacktestingConfigurationModel();
        static TickDataFileList _backtestingTickDataFileList = new TickDataFileList();
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
            string folderPath = System.IO.Path.GetDirectoryName(_backtestingResponseLibraryList.FileName);
            string assemblyPath = System.IO.Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath) == false) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UnbindInitializationModels();

            _backtestingTickDataFileList.LoadPath(Properties.Settings.Default.TickDataDirectory);
            _backtestingResponseLibraryList.LoadResponseListFromFileName(Properties.Settings.Default.ResponseFileName);

            BacktestPlotter.AxisGrid.DrawHorizontalMinorTicks = false;
            BacktestPlotter.AxisGrid.DrawHorizontalTicks = false;
            BacktestPlotter.AxisGrid.DrawVerticalMinorTicks = false;
            BacktestPlotter.AxisGrid.DrawVerticalTicks = false;

            BindInitializationModels();
        }

        #region Backtesting GUI Event Handlers

        private void BacktestingDataDirButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TickFiles|" + TikConst.WILDCARD_EXT + "|AllFiles|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _backtestingTickDataFileList.LoadPathFromFileName(openFileDialog.FileName);
            }
        }

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
            var responseName = _backtestingResponseLibraryList.SelectedResponse;
            var fileName = _backtestingResponseLibraryList.FileName;
            if (responseName != ResponseLibraryList._header)
            {
                _backtestingConfigurationModel.SelectedResponse = ResponseLoader.FromDLL(responseName, fileName);
            }
            BindResponseModels();
        }

        private void RunBacktestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UnbindInitializationModels();
            _activityModel.Messages.Clear();
            _activityModel.Status = "Backtest started.";
            var tickDataFileNames = _backtestingTickDataFileList.Where(f => f.Checked).Select(f => f.LongFileName).ToList();

            if (_reportModel != null)
            {
                _reportModel.Dispose();
            }
            if (_backtestingModel != null)
            {
                _backtestingModel.Dispose();
            }

            _backtestingModel = new BacktestingModel();

            Action updatePlots = () => { UpdateBacktestPlots(); }; 
            _reportModel = new BatchReportModel(_backtestingModel, _activityModel, updatePlots);

            _reportModel.RegisterDispatcher(DispatchableType.Fill, FillsTab.Dispatcher);
            _reportModel.RegisterDispatcher(DispatchableType.Indicator, IndicatorTab.Dispatcher);
            _reportModel.RegisterDispatcher(DispatchableType.Message, MessagesTab.Dispatcher);
            _reportModel.RegisterDispatcher(DispatchableType.Order, OrdersTab.Dispatcher);
            _reportModel.RegisterDispatcher(DispatchableType.Plot, PlotsTab.Dispatcher);
            _reportModel.RegisterDispatcher(DispatchableType.Position, PositionTab.Dispatcher);

            _backtestingModel.LoadResponse(_backtestingConfigurationModel.SelectedResponse);
            _backtestingModel.LoadTickData(tickDataFileNames);

            BindInitializationModels();

            UnbindReportModels();
            _backtestingModel.Play(_backtestingConfigurationModel.SelectedPlayToValue);
            BindReportModels();
        }

        private void BacktestingPlotUpdateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateBacktestPlots();
        }

        #endregion

        private void BindInitializationModels()
        {
            BacktestingResponseExpander.DataContext = _backtestingResponseLibraryList;
            BacktestingTickDataExpander.DataContext = _backtestingTickDataFileList;
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
            MessagesDataGrid.DataContext = _activityModel;
            BacktestingTabControl.DataContext = _reportModel;
            BacktestingPlotExpander.DataContext = _reportModel;
            _reportModelsUnbound = false;
        }

        private void UnbindReportModels()
        {
            _reportModelsUnbound = true;
            MessagesDataGrid.DataContext = null;
            BacktestingTabControl.DataContext = null;
            BacktestingPlotExpander.DataContext = null;
        }

        #region Private Backtesting Methods

        private void UpdateBacktestPlots()
        {
            BacktestPlotter.Children.RemoveAll(typeof(LineGraph));

            double maxTime = double.MinValue;
            double minTime = double.MaxValue;
            double maxValue = double.MinValue;
            double minValue = double.MaxValue;

            foreach (var plot in _reportModel.Plots.Where(p => p.Selected))
            {
                var cleanedData = plot.PlotPoints.GroupBy(point => point.Time).Select(group => group.First()).OrderBy(point => point.Time).ToList();

                // Viewport info:
                maxTime = Math.Max(DateAxis.ConvertToDouble(cleanedData.Max(point => point.Time)), maxTime);
                minTime = Math.Min(DateAxis.ConvertToDouble(cleanedData.Min(point => point.Time)), minTime);
                maxValue = Math.Max(Convert.ToDouble(cleanedData.Max(point => point.Value)), maxValue);
                minValue = Math.Min(Convert.ToDouble(cleanedData.Min(point => point.Value)), minValue);

                var dataSource = CreateBacktestPlottDataSource(cleanedData);
                BacktestPlotter.AddLineGraph(dataSource, plot.PointColor, 1, plot.Label);
            }

            var viewWidth = maxTime - minTime;
            var viewHeight = maxValue - minValue;
            BacktestPlotter.Viewport.Domain = new DataRect(minTime, minValue, viewWidth, viewHeight);
        }

        private EnumerableDataSource<TimePlotPoint> CreateBacktestPlottDataSource(IEnumerable<TimePlotPoint> plots)
        {
            var ds = new EnumerableDataSource<TimePlotPoint>(plots);
            ds.SetXMapping(p => DateAxis.ConvertToDouble(p.Time));
            ds.SetYMapping(p => Convert.ToDouble(p.Value));
            return ds;
        }

        #endregion

        private void BacktestingRunExpander_Expanded(object sender, RoutedEventArgs e)
        {

        }
    }
}
