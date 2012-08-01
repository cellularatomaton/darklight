using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TradeLink.API;

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

        public IEnumerable<PlayTo> PlayToValues
        {
            get
            {
                var values = Enum.GetValues(typeof(PlayTo)).Cast<PlayTo>();
                SelectedPlayToValue = PlayTo.End;
                return values;
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
