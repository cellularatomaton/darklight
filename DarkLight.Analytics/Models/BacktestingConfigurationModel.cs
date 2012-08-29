using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TradeLink.API;
using DarkLight.Utilities;
using TradeLink.Common;

namespace DarkLight.Analytics.Models
{
    class BacktestingConfigurationModel : INotifyPropertyChanged
    {
        private PlayTo _selectedPlayToValue = PlayTo.End;
        public PlayTo SelectedPlayToValue
        {
            get { return _selectedPlayToValue; }
            set
            {
                if (value != _selectedPlayToValue)
                {
                    _selectedPlayToValue = value;
                    NotifyPropertyChanged("SelectedPlayToValue");
                }
            }
        }

        private int _cacheWait = 10000;
        public int CacheWait
        {
            get { return _cacheWait; }
            set
            {
                if (value != _cacheWait)
                {
                    _cacheWait = value;
                    NotifyPropertyChanged("CacheWait");
                }
            }
        }

        public IEnumerable<PlayTo> PlayToValues
        {
            get
            {
                var values = Enum.GetValues(typeof(PlayTo)).Cast<PlayTo>();
                SelectedPlayToValue = PlayTo.End;
                return values;
            }
        }

        string _responsePath;
        public string ResponsePath
        {
            get { return _responsePath; }
            set
            {
                if (value != _responsePath)
                {
                    _responsePath = value;
                    NotifyPropertyChanged("ResponsePath");
                }
            }
        }

        string _responseName;
        public string ResponseName
        {
            get { return _responseName; }
            set
            {
                if (value != _responseName)
                {
                    _responseName = value;
                    NotifyPropertyChanged("ResponseName");
                }
            }
        }

        Response _selectedResponse;
        public Response SelectedResponse
        {
            get { return _selectedResponse; }
            set
            {
                if (value != _selectedResponse)
                {
                    _selectedResponse = value;
                    NotifyPropertyChanged("SelectedResponse");
                }
            }
        }

        public Response GetFreshResponseInstance()
        {
            var freshResponse = ResponseLoader.FromDLL(ResponseName, ResponsePath);
            PlottingUtilities.CopyParameters(SelectedResponse,freshResponse);
            return freshResponse;
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
