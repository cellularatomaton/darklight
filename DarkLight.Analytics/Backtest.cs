using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DarkLight.Analytics.Models;
using DarkLight.Utilities;

namespace DarkLight.Analytics
{
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
                    CopyPlotSelection(_selectedReport, value);
                    _selectedReport = value;
                    NotifyPropertyChanged("SelectedReport");
                }
            }
        }

        private ObservableCollection<BatchReportModel> _backtestReports = new ObservableCollection<BatchReportModel>();

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

        public static void CopyPlotSelection(BatchReportModel oldReport, BatchReportModel newReport)
        {
            if(oldReport != null && newReport != null)
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
            Dictionary<string,List<decimal>> plotCollections = new Dictionary<string, List<decimal>>();
            foreach (var _report in BacktestReports)
            {
                foreach (var _plot in _report.Plots)
                {
                    if(!plotCollections.ContainsKey(_plot.Label))
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