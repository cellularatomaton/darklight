using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;

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

        private int _numberTestsToRun = 0;
        public int NumberTestsToRun
        {
            get { return _numberTestsToRun; }
            set
            {
                if (value != _numberTestsToRun)
                {
                    _numberTestsToRun = value;
                    PercentComplete = Convert.ToInt32(Math.Round((Convert.ToDouble(_numberTestsCompleted) / Convert.ToDouble(_numberTestsToRun)) * 100.0));
                    NotifyPropertyChanged("NumberTestsToRun");
                }
            }
        }

        private int _numberTestsCompleted = 0;
        public int NumberTestsCompleted
        {
            get { return _numberTestsCompleted; }
            set
            {
                if (value != _numberTestsCompleted)
                {
                    _numberTestsCompleted = value;
                    PercentComplete = Convert.ToInt32(Math.Round((Convert.ToDouble(_numberTestsCompleted) / Convert.ToDouble(_numberTestsToRun)) * 100.0));
                    if(_numberTestsCompleted == _numberTestsToRun)
                    {
                        _allRunsCompleted.Set();
                    }
                    NotifyPropertyChanged("NumberTestsCompleted");
                }
            }
        }

        private AutoResetEvent _allRunsCompleted = new AutoResetEvent(false);
        public AutoResetEvent AllRunsCompleted 
        {
            get { return _allRunsCompleted; }
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
