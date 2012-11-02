using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using DarkLight.Utilities;
using TradeLink.API;
using TradeLink.AppKit;

namespace DarkLight.Analytics.Models
{
    //public class ResultsModel : INotifyPropertyChanged
    //{

    //    private ObservableCollection<DarkLightResults> _reportResults = new ObservableCollection<DarkLightResults>();
    //    public ObservableCollection<DarkLightResults> ReportResults
    //    {
    //        get { return _reportResults; }
    //        set
    //        {
    //            if (value != _reportResults)
    //            {
    //                _reportResults = value;
    //                NotifyPropertyChanged("ReportResults");
    //            }
    //        }
    //    }

    //    private DarkLightResults _selectedResult;
    //    public DarkLightResults SelectedResult
    //    {
    //        get { return _selectedResult; }
    //        set
    //        {
    //            if (value != _selectedResult)
    //            {
    //                _selectedResult = value;
    //                NotifyPropertyChanged("SelectedResult");
    //            }
    //        }
    //    }

    //    public ResultsModel()
    //    {
    //    }

    //    public void Clear()
    //    {
    //        ReportResults.Clear();
    //    }

    //    public void AddResults(string name, List<Trade> trades, DebugDelegate gotDebug, decimal riskFreeRate = .01m, decimal commissionPerShare = .01m)
    //    {
    //        var results = new DarkLightResults(DarkLightResults.GetResults(name, trades, gotDebug, riskFreeRate, commissionPerShare));
    //        results.PropertyChanged += (sender, args) =>
    //        {
    //            if(args.PropertyName == "Selected")
    //            {
    //                SelectedResult = sender as DarkLightResults;
    //            }
    //        };
    //        ReportResults.Add(results);
    //    }

    //    #region INotifyPropertyChanged
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    protected void NotifyPropertyChanged(String info)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(info));
    //        }
    //    }
    //    #endregion
    //}
}
