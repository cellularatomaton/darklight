using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using DarkLight.Events;
using DarkLight.Infrastructure;
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

        //IMediator _mediator;
        IBacktestAdapter _backTestAdapter;        

        #endregion

        public MockBacktestService(IBacktestAdapter backTestAdapter)
        {
            _backTestAdapter = backTestAdapter;          
            _backTestAdapter.OnRunBacktest += RunBackTest;
        }

        public void RunBackTest(IHistDataService _histDataService, DarkLightResponse _response)
        {
            Task.Factory.StartNew(() =>
            {
                //Begin
                _backTestAdapter.Publish(new StatusEvent
                {
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

                    _backTestAdapter.Publish(new StatusEvent
                    {
                        Key = "",
                        StatusType = StatusType.Progress,
                        ProgressModels = progressModels
                    });                     
                }

                //Complete
                _backTestAdapter.Publish(new StatusEvent
                {
                    Key = "",
                    StatusType = StatusType.Complete
                });
          });

        }

        public BacktestRecord GetBackTest(string backtestID)
        {
            return new BacktestRecord();
        }

    }
}