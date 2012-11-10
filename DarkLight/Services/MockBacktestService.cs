using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
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

        #endregion

        public MockBacktestService()
        {
        }

        public string RunBackTest(IHistDataService _histDataService, DarkLightResponse _response)
        {
            return "";
        }

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
                            Slot = "Slot " + (j+1).ToString(),
                            BacktestID = "Backtest xxxx",
                            ProgressValue = d / 100
                        };

                        _statusViewModel.ProgressModels[j] = progressModel;
                    }
                    /*
                    _statusViewModel.ProgressModels[0].ProgressValue += 0.1;
                    _statusViewModel.ProgressModels[1].ProgressValue += 0.1;
                    _statusViewModel.ProgressModels[2].ProgressValue += 0.1;
                    _statusViewModel.ProgressModels[3].ProgressValue += 0.1;
                    */
                    //_statusViewModel.ProgressView.CollectionChanged(
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
