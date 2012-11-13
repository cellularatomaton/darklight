using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using DarkLight.Events;
using DarkLight.Interfaces;
using System.Threading.Tasks;
using DarkLight.Backtest.Models;
using DarkLight.Customizations;
using DarkLight.Backtest.ViewModels;
using System.Threading;

namespace DarkLight.Services
{
    class MockBacktestService : IBacktestService
    {
        #region Members

        BacktestStatusViewModel _statusViewModel;
        IEventAggregator _eventAggregator;

        #endregion

        public MockBacktestService(IEventAggregator eventBroker)
        {
            _eventAggregator = eventBroker;
        }

        //Version 1: implement using Event Aggregator
        public string RunBackTest(IHistDataService _histDataService, DarkLightResponse _response)
        {
            Task.Factory.StartNew(() =>
            {
                //Begin
                _eventAggregator.Publish(new StatusEvent
                {
                    ServiceType = ServiceType.Backtest,
                    Key = "",
                    StatusType = StatusType.Begin
                });

                //Progress
                for (int i = 0; i <= 100; i++)
                {
                    Thread.Sleep(100);
                    double d = (double)i;

                    var progressModels = new BacktestProgressModel[4];
                    for (int j = 0; j < 4; j++)
                    {
                        progressModels[j] = new BacktestProgressModel
                                                {
                                                    Slot = "Slot " + j.ToString() + ":",
                                                    BacktestID = "Momentum " + j.ToString(),
                                                    ProgressValue = d / 100
                                                };
                    }

                    _eventAggregator.Publish(new StatusEvent
                    {
                        ServiceType = ServiceType.Backtest,
                        Key = "",
                        StatusType = StatusType.Progress,
                        ProgressModels = progressModels
                    });
                }

                //Complete
                _eventAggregator.Publish(new StatusEvent
                {
                    ServiceType = ServiceType.Backtest,
                    Key = "",
                    StatusType = StatusType.Complete
                });
            });

            return "";
        }

        //Version 2: implement using direct reference to viewmodel
        public string RunBackTest(IHistDataService _histDataService, DarkLightResponse _response, BacktestStatusViewModel viewModel)
        {
            _statusViewModel = viewModel;

            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(100);
                    double d = (double)i;

                    for (int j = 0; j < 4; j++)
                    {
                        var progressModel = new BacktestProgressModel
                        {
                            Slot = "Slot " + (j + 1).ToString(),
                            BacktestID = "Momentum " + j.ToString(),
                            ProgressValue = d / 100
                        };

                        //NOTE: here we are replacing the entire model rather than updating individual progress value.
                        //In order to update individual values we could implement NotifyPropertyChanged on individual properties in ProgressModel
                        //Additionally, trying a update a view (even if based on a BindableCollection) from separate thread like this does not work 
                        _statusViewModel.ProgressModels[j] = progressModel;
                    }
                }
            });

            return "";
        }

        public BacktestRecord GetBackTest(string backtestID)
        {
            return new BacktestRecord();
        }

    }
}