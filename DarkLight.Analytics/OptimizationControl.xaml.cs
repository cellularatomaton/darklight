﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using DarkLight.Analytics.Models;
using DarkLight.Utilities;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Win32;
using TradeLink.API;
using TradeLink.AppKit;
using TradeLink.Common;

namespace DarkLight.Analytics
{
    /// <summary>
    /// Interaction logic for OptimizationControl.xaml
    /// </summary>
    public partial class OptimizationControl : UserControl
    {
        // Shared Models:
        static ActivityModel _activityModel = new ActivityModel();

        // Optimization Models:
        static OptimizationModel _optimizationModel;
        static OptimizationConfigurationModel _optimizationConfigurationModel = new OptimizationConfigurationModel();
        static TickDataFileList _optimizationTickDataFileList = new TickDataFileList();
        static ResponseLibraryList _optimizationResponseLibraryList = new ResponseLibraryList();

        static List<PlottableValue<DarkLightResults>> _optimizationResults = new List<PlottableValue<DarkLightResults>>();
        static AutoResetEvent _optimizationResetEvent = new AutoResetEvent(false);

        bool _initializationModelsUnbound = false;
        bool _optimizationModelsUnbound = false;
        bool _responseModelsUnbound = false;

        public OptimizationControl()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += LoadFromResponseFolder;

            InitializeComponent();
        }

        private Assembly LoadFromResponseFolder(object sender, ResolveEventArgs args)
        {
            string folderPath = System.IO.Path.GetDirectoryName(_optimizationResponseLibraryList.FileName);
            string assemblyPath = System.IO.Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath) == false) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UnbindInitializationModels();

            _optimizationTickDataFileList.LoadPath(Properties.Settings.Default.TickDataDirectory);
            _optimizationResponseLibraryList.LoadResponseListFromFileName(Properties.Settings.Default.ResponseFileName);

            OptimizationPlotter.AxisGrid.DrawHorizontalMinorTicks = false;
            OptimizationPlotter.AxisGrid.DrawHorizontalTicks = false;
            OptimizationPlotter.AxisGrid.DrawVerticalMinorTicks = false;
            OptimizationPlotter.AxisGrid.DrawVerticalTicks = false;

            _optimizationConfigurationModel.PopulatePlottableValues();

            BindInitializationModels();
        }

        #region Optimization GUI Event Handlers

        private void OptimizationDataDirButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TickFiles|" + TikConst.WILDCARD_EXT + "|AllFiles|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _optimizationTickDataFileList.LoadPathFromFileName(openFileDialog.FileName);
            }
        }

        private void OptimizationResponseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Responses Libraries|*.dll|AllFiles|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _optimizationResponseLibraryList.LoadResponseListFromFileName(openFileDialog.FileName);
            }
        }

        private void ResponseComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_initializationModelsUnbound ||
                _responseModelsUnbound)
            {
                return;
            }

            UnbindResponseModels();
            var responseName = _optimizationResponseLibraryList.SelectedResponse;
            var fileName = _optimizationResponseLibraryList.FileName;
            if (responseName != ResponseLibraryList._header)
            {
                _optimizationConfigurationModel.SelectedResponse = ResponseLoader.FromDLL(responseName, fileName);
            }
            _optimizationConfigurationModel.PopulateAdjustableProperties();
            BindResponseModels();
        }

        private void AdjustableProperty1ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void OptimizationButton_Click(object sender, RoutedEventArgs e)
        {
            Task optimizationManagementTask = new Task(() =>
            {
                _activityModel.Optimizing = true;
                int numberUniformSamples = _optimizationConfigurationModel.NumberUniformSamples;
                //int runCount = 1;
                _activityModel.PercentComplete = 0;
                _optimizationResults.Clear();

                Action<string> onStatusUpdate = s =>
                {
                    _activityModel.Status = s;
                };

                _optimizationModel = new OptimizationModel(onStatusUpdate);
                BindOptimizationModel();
                Action<Response, double> doWork = (response, d) =>
                {
                    RunOptimizationModel(response, d);
                    _optimizationResetEvent.WaitOne(30000);
                };
                RunOptimization1D(numberUniformSamples, doWork);
                _activityModel.Optimizing = false;
            });
            optimizationManagementTask.Start();
        }

        private void OptimizationPlotUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateOptimizationPlot();
        }

        #endregion

        #region Data Context Binding

        private void BindInitializationModels()
        {
            OptimizationResponseExpander.DataContext = _optimizationResponseLibraryList;
            OptimizationTickDataExpander.DataContext = _optimizationTickDataFileList;
            OptimizationRunExpander.DataContext = _optimizationConfigurationModel;
            OptimizationPlotConfigurationListBox.DataContext = _optimizationConfigurationModel;
            OptimizationStatusBar.DataContext = _activityModel;
            _initializationModelsUnbound = false;
        }

        private void UnbindInitializationModels()
        {
            _initializationModelsUnbound = true;
            OptimizationResponseExpander.DataContext = null;
            OptimizationTickDataExpander.DataContext = null;
            OptimizationRunExpander.DataContext = null;
            OptimizationPlotConfigurationListBox.DataContext = null;
            OptimizationStatusBar.DataContext = null;
        }

        private void BindResponseModels()
        {
            OptimizationResponsePropertyGrid.DataContext = _optimizationConfigurationModel.SelectedResponse;
            OptimizationInputExpander.DataContext = _optimizationConfigurationModel;
            OptimizationOutputExpander.DataContext = _optimizationConfigurationModel;
            _responseModelsUnbound = false;
        }

        private void UnbindResponseModels()
        {
            _responseModelsUnbound = true;
            OptimizationResponsePropertyGrid.DataContext = null;
            OptimizationInputExpander.DataContext = null;
            OptimizationOutputExpander.DataContext = null;
        }

        private void BindOptimizationModel()
        {
            OptimizationTabControl.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                OptimizationTabControl.DataContext = _optimizationModel;
            }));
            _optimizationModelsUnbound = false;
        }

        private void UnbindOptimizationModel()
        {
            _optimizationModelsUnbound = true;
            OptimizationTabControl.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                OptimizationTabControl.DataContext = null;
            }));
        }

        #endregion

        #region Private Optimization Methods

        private void UpdateOptimizationPlot()
        {
            OptimizationPlotter.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                OptimizationPlotter.Children.RemoveAll(typeof(LineGraph));
                
                double maxX = double.MinValue;
                double minX = double.MaxValue;
                double maxY = double.MinValue;
                double minY = double.MaxValue;

                if (_optimizationConfigurationModel.PlottableValues.Any(p => p.Selected))
                {
                    var plottableValues = _optimizationConfigurationModel.PlottableValues.Where(p => p.Selected).ToList();
                    var colorList = PlottingUtilities.GetColorList(plottableValues.Count);
                    for (int i = 0; i < plottableValues.Count; i++)
                    {
                        plottableValues[i].PlotColor = colorList[i];
                    }

                    foreach (var plottableValue in plottableValues)
                    {
                        var pointList = PlottingUtilities.ToPlottable(_optimizationResults, plottableValue);
                        var dataSource = CreateResultsDataSource(pointList.OrderBy(p => p.X));
                        OptimizationPlotter.AddLineGraph(dataSource, plottableValue.PlotColor, 1,
                                                            plottableValue.PropertyName);

                        maxX = Math.Max(pointList.Max(r => r.X), maxX);
                        minX = Math.Min(pointList.Min(r => r.X), minX);
                        maxY = Math.Max(pointList.Max(r => r.Y), maxY);
                        minY = Math.Min(pointList.Min(r => r.Y), minY);
                    }
                }
                else
                {
                    var plottableValue = _optimizationConfigurationModel.PlottableValues[0];
                    var pointList = PlottingUtilities.ToPlottable(_optimizationResults, plottableValue);
                    var dataSource = CreateResultsDataSource(pointList.OrderBy(p => p.X));
                    OptimizationPlotter.AddLineGraph(dataSource, Colors.GreenYellow, 1,
                                                     plottableValue.PropertyName);

                    maxX = Math.Max(pointList.Max(r => r.X), maxX);
                    minX = Math.Min(pointList.Min(r => r.X), minX);
                    maxY = Math.Max(pointList.Max(r => r.Y), maxY);
                    minY = Math.Min(pointList.Min(r => r.Y), minY);
                }

                var viewWidth = maxX - minX;
                var viewHeight = maxY - minY;
                OptimizationPlotter.Viewport.Domain = new DataRect(minX, minY, viewWidth, viewHeight);
            }));
        }

        private EnumerableDataSource<PlottablePoint> CreateResultsDataSource(IEnumerable<PlottablePoint> plottablePoints)
        {
            var ds = new EnumerableDataSource<PlottablePoint>(plottablePoints);
            ds.SetXMapping(p => p.X);
            ds.SetYMapping(p => p.Y);
            return ds;
        }

        private void RunOptimization1D(int numberUniformSamples, Action<Response, double> doWork)
        {
            var adjustableProperty = _optimizationConfigurationModel.AdjustableProperty1;
            foreach (var val in adjustableProperty.GetRange(numberUniformSamples))
            {
                adjustableProperty.CurrentValue = val;
                adjustableProperty.SetCurrentValue(_optimizationConfigurationModel.SelectedResponse);
                doWork(_optimizationConfigurationModel.SelectedResponse, Convert.ToDouble(val));
            }
            _optimizationModel.Dispose();
        }

        private void RunOptimizationModel(Response response, double xValue)
        {
            var tickDataFileNames = _optimizationTickDataFileList.Where(f => f.Checked).Select(f => f.LongFileName).ToList();
            _optimizationModel.Reset();
            _optimizationModel.LoadResponse(response);
            _optimizationModel.LoadTickData(tickDataFileNames);
            Action<Results> onCompleted = r =>
            {
                var darkLightResults = new DarkLightResults(r);
                _optimizationResults.Add(new PlottableValue<DarkLightResults> { X = xValue, Value = darkLightResults });
                _optimizationResetEvent.Set();
                UpdateOptimizationPlot();
            };
            Action<int> onPercentComplete = i =>
            {
                _activityModel.PercentComplete = i;
            };
            _optimizationModel.Play(_optimizationConfigurationModel.SelectedPlayToValue, onCompleted, onPercentComplete);
        }

        #endregion

        
    }
}
