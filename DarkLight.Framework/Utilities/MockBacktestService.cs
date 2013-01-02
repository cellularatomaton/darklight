using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using System.Threading.Tasks;
using System.Threading;
using DarkLight.Framework.Data.Backtest;
using DarkLight.Framework.Data.Common;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Adapters;
using DarkLight.Framework.Interfaces.CEP;
using DarkLight.Framework.Interfaces.Services;
using com.espertech.esper.client;

namespace DarkLight.Framework.Utilities
{
    public class MockBacktestService : IBacktestService, DarkLight.Framework.Interfaces.CEP.IHandle<BacktestRequestEvent>
    {
        #region Members

        const int numSlots = 4;
        IEventBroker _broker;
        Dictionary<string, ManualResetEventSlim> _groupDict; 

        #endregion

        public MockBacktestService(IEventBroker broker)
        {
            _broker = broker;
            _groupDict = new Dictionary<string, ManualResetEventSlim>();                        
        }

        public void Initialize()
        {
            //TODO: refactor
            _broker.Subscribe(this);
        }

        public void RunBackTest(IHistDataService _histDataService, DarkLightResponse _response)
        {
            Task.Factory.StartNew(() => {

                var progressModels = new BacktestProgressModel[numSlots];
                string key = _response.Key;
                Random random = new Random();
                int currentTestIndex = -1;
                int[] slotTestIndex = Enumerable.Range(0, numSlots).ToArray();
                double[] slotSpeed = new double[numSlots];
                double[] slotProgress = new double[numSlots];
                int numFinishedSlots = 0;
                ManualResetEventSlim mres = new ManualResetEventSlim(true);
                _groupDict.Add(key, mres);

                var testList = MockUtilities.GenerateResponseSessionRecords(key);

                _broker.Publish(new StatusEvent
                {
                    Key = key,
                    StatusType = StatusType.Begin
                });

                while (true)
                {
                    mres.Wait();
                    Thread.Sleep(100);

                        for (int s = 0; s < numSlots; s++)
                        {
                            if (slotProgress[s] < 0.0001 || slotProgress[s] >= 1.0)                               
                            {
                                if (currentTestIndex < testList.Count - 1)
                                {
                                    currentTestIndex++;
                                    slotTestIndex[s] = currentTestIndex;
                                    slotSpeed[s] = (double)random.Next(1, 8) / 100;
                                    slotProgress[s] = 0;
                                }
                                else
                                {
                                    numFinishedSlots++;
                                    slotSpeed[s] = 0;
                                }
                            }

                            slotProgress[s] += slotSpeed[s];
                            var testIndex = slotTestIndex[s];
                            var test = testList[testIndex];

                            progressModels[s] = new BacktestProgressModel
                                                    {
                                                        Slot = "Slot " + (s + 1).ToString() + ":",
                                                        BacktestID = test.GUID,
                                                        ProgressValue = slotProgress[s]
                                                    };
                        }

                        _broker.Publish(new StatusEvent
                        {
                            Key = key,
                            StatusType = StatusType.Progress,
                            ProgressModels = progressModels,
                            NumBacktestsComplete = currentTestIndex + 1,
                            NumBacktests = testList.Count
                        });

                        if (numFinishedSlots == numSlots)
                        {
                            _broker.Publish(new StatusEvent
                            {
                                Key = "",
                                StatusType = StatusType.Complete
                            });
                            return;
                        }
                    }                    
                                          
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

        public void Handle(BacktestRequestEvent request)
        {
            if (request.ActionType == ServiceAction.Run)
                RunBackTest(request.HistDataService, request.Response);
            else if (request.ActionType == ServiceAction.Pause)
                PauseBackTest(request.Key);
            else if (request.ActionType == ServiceAction.Resume)
                ResumeBackTest(request.Key);
        }

        public void Update(object sender, UpdateEventArgs e)
        {

        }
    }
}