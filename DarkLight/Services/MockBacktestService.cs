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
using DarkLight.Utilities;

namespace DarkLight.Services
{
    class MockBacktestService : IBacktestService
    {
        #region Members

        //IMediator _mediator;
        IBacktestAdapter _backTestAdapter;
        Dictionary<string, ManualResetEventSlim> _groupDict; 

        #endregion

        public MockBacktestService(IBacktestAdapter backTestAdapter)
        {
            _backTestAdapter = backTestAdapter;          
            _backTestAdapter.OnRunBacktest += RunBackTest;
            _backTestAdapter.OnPauseBacktest += PauseBackTest;
            _backTestAdapter.OnResumeBacktest += ResumeBackTest;
            _backTestAdapter.OnCancelBacktest += CancelBackTest;
            _groupDict = new Dictionary<string, ManualResetEventSlim>();
        }

        public void RunBackTest(IHistDataService _histDataService, DarkLightResponse _response)
        {
            string key = _response.Key;
            var groupRecord = MockUtilities.ParseGroupGUID(key);
            var parameters = groupRecord.ParameterSpace.Split(new char[] {'|'});
            
            var xma0 = parameters[0];
            var xma1 = xma0.Split(new char[] {'['});
            var xma2 = xma1[1].Split(new char[] { ']' });
            var xma3 = xma2[0].Split(new char[] { ',' });
            int xmaMin = Convert.ToInt32(xma3[0]);
            int xmaMax = Convert.ToInt32(xma3[1]);
            int numXma = xmaMax - xmaMin + 1;

            var interval0 = parameters[1];
            var interval1 = interval0.Split(new char[] { '[' });
            var interval2 = interval1[1].Split(new char[] { ']' });
            var interval3 = interval2[0].Split(new char[] { ',' });
            int intervalMin = Convert.ToInt32(interval3[0]);
            int intervalMax = Convert.ToInt32(interval3[1]);
            int numInterval = intervalMax - intervalMin + 1;         

            var date1 = Convert.ToDateTime(parameters[parameters.Length - 2]);
            var date2 = Convert.ToDateTime(parameters[parameters.Length - 1]);
            int numDays = (int)date2.Subtract(date1).TotalDays + 1;
            int numSlotPeriods = numDays / 4;

            int numBacktests = numXma*numInterval*numDays;
            int numBacktestsComplete = 0;
            
            ManualResetEventSlim mres = new ManualResetEventSlim(true);
            _groupDict.Add(key, mres);

            Task.Factory.StartNew(() =>
            {
                //Begin
                _backTestAdapter.Publish(new StatusEvent
                {
                    Key = key,
                    StatusType = StatusType.Begin
                });

                //Progress
                var statusEvents = new List<StatusEvent>();

                for (int x = 0; x <= numXma; x++)
                {
                    string xma = (xmaMin + x).ToString();
                    for (int i = 0; i <= numInterval; i++)
                    {
                        string interval = (intervalMin + i).ToString();

                        for (int t = 0; t < numSlotPeriods; t++)
                        {
                            string[] currentDates = new string[4];
                            var currentStartDate = date1.AddDays(t*4);
                            for (int tt = 0; tt < 4; tt++)
                            {
                                currentDates[tt] = currentStartDate.AddDays(tt).ToString("MM/dd/yyy");
                            }

                            for (int z = 0; z < 10; z++)
                            {
                                double d = (double) z;

                                var progressModels = new BacktestProgressModel[4];
                                for (int j = 0; j < 4; j++)
                                {
                                    progressModels[j] = new BacktestProgressModel
                                                            {
                                                                Slot = "Slot " + (j + 1).ToString() + ":",
                                                                BacktestID =
                                                                    groupRecord.ResponseType + "|XMACoefficient=" + xma +
                                                                    "|IntervalSize=" + interval + "|" + currentDates[j],
                                                                ProgressValue = d/10
                                                            };
                                }                                

                                statusEvents.Add(new StatusEvent
                                                     {
                                                         Key = key,
                                                         StatusType = StatusType.Progress,
                                                         ProgressModels = progressModels,
                                                         NumBacktestsComplete = numBacktestsComplete,
                                                         NumBacktests = numBacktests
                                                     });
                            }

                            numBacktestsComplete += 4;
                        }
                    }
                }

                foreach (var statusEvent in statusEvents)
                {
                    mres.Wait();
                    Thread.Sleep(100);
                    _backTestAdapter.Publish(statusEvent);        
                }                                                 

                //Complete
                _backTestAdapter.Publish(new StatusEvent
                {
                    Key = "",
                    StatusType = StatusType.Complete
                });
          });

        }

        public void PauseBackTest(string key)
        {
            if (_groupDict.ContainsKey(key))
                _groupDict[key].Reset();
        }

        public void ResumeBackTest(string key)
        {
            if (_groupDict.ContainsKey(key))
                _groupDict[key].Set();
        }

        public void CancelBackTest(string backtestID)
        {
        
        }

        public ResponseSessionRecord GetBackTest(string backtestID)
        {
            return new ResponseSessionRecord();
        }

    }
}