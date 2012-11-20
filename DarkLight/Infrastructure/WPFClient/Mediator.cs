﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using DarkLight.Events;
using DarkLight.Infrastructure.Adapters;


namespace DarkLight.Infrastructure
{
    public class Mediator : IMediator
    { 
        #region Private Members

        IEventAggregator _eventAggregator;
        IMediatorAdapter _mediatorAdapter;

        readonly double _budget = 50;
        PriorityTaskScheduler _preemptiveScheduler;

        //TODO: implement 
        private readonly Dictionary<object, Task<object>> _baseQueue = new Dictionary<object, Task<object>>();
        private readonly TaskFactory _backgroundTaskFactory;
        private readonly TaskFactory _staTaskFactory;

        #endregion

        #region Constructors

        public Mediator(IMediatorAdapter mediatorAdapter)
        {
            _mediatorAdapter = mediatorAdapter;
            _mediatorAdapter.OnBroadcast += Broadcast;
            _eventAggregator = IoC.Get<IEventAggregator>(); 
           _preemptiveScheduler = new PriorityTaskScheduler();
           _backgroundTaskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Implementation of IMediator

        public void Broadcast(DarkLightEvent darkLightEvent)
        {
            //Test w/ everything periodic for now
            //var taskType = TaskType.Periodic;           
            object target = new object();
            object[] message = new object[1];
            System.Action<object[]> action = null;

            switch (darkLightEvent.EventType)//taskType)
            //TODO: refactor mapping below
            {
                //NOT BEING USED YET
                case EventType.BacktestRequest://TaskType.Background:
                    // check if already running
                    if (!_baseQueue.ContainsKey(target))
                   {
                       var bgTask = _backgroundTaskFactory.StartNew(() => _mediatorAdapter.Publish(darkLightEvent));
                        //_baseQueue.Add(target, bgTask);
                    }
                    break;

                case EventType.Status://TaskType.Periodic:
                    var periodicTask = new PeriodicTask(() => _eventAggregator.Publish(darkLightEvent), _budget);
                    periodicTask.Start(_preemptiveScheduler);
                    break;
                /*
                case EventType.BacktestRequest://TaskType.Sporadic:
                    // one Periodic to run all sporadics
                    var sporadicTask1 = new SporadicTask(() => _mediatorAdapter.Publish(darkLightEvent), _budget);
                    sporadicTask1.Start(_preemptiveScheduler);
                    break;
                */
                case EventType.LinkedNavigation:
                    // one Periodic to run all sporadics
                    var sporadicTask2 = new SporadicTask(() => _eventAggregator.Publish(darkLightEvent), _budget);
                    sporadicTask2.Start(_preemptiveScheduler);
                    break;

                //NOT BEING USED YET
                case EventType.Result://TaskType.LongRunning:
                    //UI
                    _staTaskFactory.StartNew(() => action.DynamicInvoke(message));
                    break;
            }
        }

        #endregion

        /*
        public void Register()
        {            
        }

        public void Unregister()
        {
            
        }
        */ 
    }
}
