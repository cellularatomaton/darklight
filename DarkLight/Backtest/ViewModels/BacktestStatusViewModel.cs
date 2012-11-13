using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using DarkLight.Customizations;
using DarkLight.Events;
using System.Collections;
using DarkLight.Interfaces;
using DarkLight.Services;
using DarkLight.Backtest.Models;
using System.ComponentModel;
using System.Windows.Data;

namespace DarkLight.Backtest.ViewModels
{
    public class BacktestStatusViewModel : DarkLightScreen, IHandle<StatusEvent>
    {
        #region Private Members

        int _numBacktestSlots;
        double _totalProgressValue;
        string _backtestStatus;
        string _backtestName;
        string _totalProgressString;

        #endregion

        #region Public Members

        public string BacktestStatus
        {
            get { return _backtestStatus; }
            set
            {
                _backtestStatus = value;
                NotifyOfPropertyChange(() => BacktestStatus);
            }
        }

        public string BacktestName
        {
            get { return _backtestName; }
            set
            {
                _backtestName = value;
                NotifyOfPropertyChange(() => BacktestName);
            }
        }

        public string TotalProgressString
        {
            get { return _totalProgressString; }
            set
            {
                _totalProgressString = value;
                NotifyOfPropertyChange(() => TotalProgressString);
            }
        }

        public double TotalProgressValue
        {
            get { return _totalProgressValue; }
            set
            {
                _totalProgressValue = value;
                NotifyOfPropertyChange(() => TotalProgressValue);
            }
        }

        private BindableCollection<BacktestProgressModel> _progressModels;
        public BindableCollection<BacktestProgressModel> ProgressModels
        {
            get { return _progressModels; }
            set
            {
                _progressModels = value;
                NotifyOfPropertyChange(() => ProgressModels);
            }
        }

        #endregion

        #region Constructors

        public BacktestStatusViewModel()
        {

        }

        #endregion

        #region Public Methods

        public void Initialize(string backtestName, int numSlots)
        {
            BacktestName = backtestName;
            _numBacktestSlots = numSlots;

            ProgressModels = new BindableCollection<BacktestProgressModel>();
            for (int i = 0; i < _numBacktestSlots; i++)
            {
                ProgressModels.Add(new BacktestProgressModel());
            }

            BacktestStatus = "Initializing Backtest:";

            IoC.Get<IEventAggregator>().Subscribe(this);
        }

        #endregion

        #region Implementation of IHandle<ServiceStatusEvent>

        public void Handle(StatusEvent se)
        {
            if (se.StatusType == StatusType.Begin)
            {
                BacktestStatus = "Running Backtest:";
            }
            else if (se.StatusType == StatusType.Progress)
            {
                //Slot Progress                
                double tempTotal = 0;
                for (int i = 0; i < ProgressModels.Count; i++)
                {
                    ProgressModels[i] = se.ProgressModels[i];
                    tempTotal += se.ProgressModels[i].ProgressValue / _numBacktestSlots;
                }

                //Total Progress
                TotalProgressString = "Percent Complete (" + (tempTotal*1000).ToString() + " / 1000 tests complete):";
                TotalProgressValue = tempTotal;
            }
            else if (se.StatusType == StatusType.Complete)
            {
                BacktestStatus = "Backtest Complete";
            }
        }

        #endregion

    }
}
