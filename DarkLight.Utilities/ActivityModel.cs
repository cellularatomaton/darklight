using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DarkLight.Analytics.Models
{
    public class ActivityModel : INotifyPropertyChanged
    {
        private bool _busy = false;
        public bool Busy
        {
            get { return _busy; }
            set
            {
                if (value != _busy)
                {
                    _busy = value;
                    NotifyPropertyChanged("Busy");
                }
            }
        }

        private bool _backtesting = false;
        public bool Backtesting
        {
            get { return _backtesting; }
            set
            {
                if (value != _backtesting)
                {
                    _backtesting = value;
                    NotifyPropertyChanged("Backtesting");
                }
            }
        }

        private bool _optimizing = false;
        public bool Optimizing
        {
            get { return _optimizing; }
            set
            {
                if (value != _optimizing)
                {
                    _optimizing = value;
                    NotifyPropertyChanged("Optimizing");
                }
            }
        }

        private int _percentComplete = 0;
        public int PercentComplete
        {
            get { return _percentComplete; }
            set
            {
                if (value != _percentComplete)
                {
                    _percentComplete = value;
                    NotifyPropertyChanged("PercentComplete");
                }
            }
        }

        private string _status = "Idle";
        public string Status
        {
            get { return _status; }
            set
            {
                if (value != _status)
                {
                    _status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        ObservableCollection<ObservableMessage> _messages = new ObservableCollection<ObservableMessage>();
        public ObservableCollection<ObservableMessage> Messages
        {
            get { return _messages; }
            set
            {
                if (value != _messages)
                {
                    _messages = value;
                    NotifyPropertyChanged("Messages");
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

    public class ObservableMessage : INotifyPropertyChanged
    {
        private string _message = string.Empty;
        public string Message
        {
            get { return _message; }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    NotifyPropertyChanged("Message");
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
