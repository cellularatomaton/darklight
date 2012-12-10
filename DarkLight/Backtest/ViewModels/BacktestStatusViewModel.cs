using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using DarkLight.Customizations;
using DarkLight.Enums;
using DarkLight.Events;
using System.Collections;
using DarkLight.Infrastructure;
using DarkLight.Infrastructure.WPFClient;
using DarkLight.Interfaces;
using DarkLight.Services;
using DarkLight.Backtest.Models;
using System.ComponentModel;
using System.Windows.Data;
using com.espertech.esper.client;

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
            var mediator = IoC.Get<IMediator>();
            if (mediator.GetType() == typeof(Mediator))
                IoC.Get<IEventAggregator>().Subscribe(this);
            else if (mediator.GetType() == typeof(MediatorCEP))
                mediator.Subscribe(Events.EventType.Status, UpdateFromCEP);
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
            IoC.Get<IMediator>().Broadcast(requestEvent);
        }

        public void ResumeBacktest()
        {
            var requestEvent = new BacktestRequestEvent();
            requestEvent.ActionType = ServiceAction.Resume;
            requestEvent.Key = BacktestName;
            IoC.Get<IMediator>().Broadcast(requestEvent);
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

        public void UpdateFromCEP(object sender, UpdateEventArgs e)
        {
            var statusEvent = (StatusEvent)e.NewEvents[0].Underlying;
            Handle(statusEvent);
        }
    }
}
