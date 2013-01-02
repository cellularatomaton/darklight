using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using DarkLight.Client.Customizations;
using System.Collections;
using DarkLight.Framework.Data.Backtest;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.CEP;
using DarkLight.Framework.Interfaces.Common;
using DarkLight.Infrastructure;
using DarkLight.Infrastructure.Mediator;
using System.ComponentModel;
using System.Windows.Data;
using com.espertech.esper.client;
using EventType = DarkLight.Framework.Enums.EventType;

namespace DarkLight.Client.Backtest.ViewModels
{
    public class BacktestStatusViewModel : DarkLightScreen, DarkLight.Framework.Interfaces.CEP.IHandle<StatusEvent>
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
            IoC.Get<IEventBroker>().Subscribe(this);
        }

        #endregion

        #region Public Methods

        public void Initialize(BacktestGroupRecord backtestGroup, int numSlots)
        {
            BacktestName = backtestGroup.GUID;
            _numBacktestSlots = numSlots;

            ProgressModels = new BindableCollection<BacktestProgressModel>();
            for (int i = 0; i < _numBacktestSlots; i++)
            {
                ProgressModels.Add(new BacktestProgressModel());
            }

            BacktestStatus = "Initializing Backtest:";
        }

        public void PauseBacktest()
        {
            var requestEvent = new BacktestRequestEvent();
            requestEvent.ActionType = ServiceAction.Pause;
            requestEvent.Key = BacktestName;
            IoC.Get<IEventBroker>().Publish(requestEvent);
        }

        public void ResumeBacktest()
        {
            var requestEvent = new BacktestRequestEvent();
            requestEvent.ActionType = ServiceAction.Resume;
            requestEvent.Key = BacktestName;
            IoC.Get<IEventBroker>().Publish(requestEvent);
        }

        #endregion

        #region Implementation of IHandle<ServiceStatusEvent>

        public void Handle(StatusEvent se)
        {
            if (se.Key == BacktestName)
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
                        //tempTotal += se.ProgressModels[i].ProgressValue / _numBacktestSlots;
                    }

                    //Total Progress
                    TotalProgressString = "Percent Complete (" + se.NumBacktestsComplete.ToString() + " / " +
                                          se.NumBacktests.ToString() + " tests complete):";
                    TotalProgressValue = ((double) se.NumBacktestsComplete)/(se.NumBacktests);
                }
                else if (se.StatusType == StatusType.Complete)
                {
                    BacktestStatus = "Backtest Complete";
                }
            }
        }

        #endregion

    }
}
