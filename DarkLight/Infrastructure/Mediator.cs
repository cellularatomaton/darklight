using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using DarkLight.Events;


namespace DarkLight.Infrastructure
{
    public class Mediator : IMediator
    { 
        #region Private Members

        IEventAggregator _eventAggregator;

        readonly double _budget = 50;
        PriorityTaskScheduler _preemptiveScheduler;

        //TODO: implement 
        private readonly Dictionary<object, Task<object>> _baseQueue = new Dictionary<object, Task<object>>();
        private readonly TaskFactory _backgroundTaskFactory;
        private readonly TaskFactory _staTaskFactory;

        #endregion

        #region Constructors

        public Mediator()
        {
            _eventAggregator = IoC.Get<IEventAggregator>(); //eventAggregator;
           _preemptiveScheduler = new PriorityTaskScheduler(); 
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Implementation of IMediator

        public void Publish(StatusEvent statusEvent)
        {
            //Test w/ everything periodic for now
            var taskType = TaskType.Periodic;
           
            object target = new object();
            object[] message = new object[1];
            System.Action<object[]> action = null;

            switch (taskType)
            {
                case TaskType.Background:
                    // check if already running
                    if (!_baseQueue.ContainsKey(target))
                    {
                        var bgTask = _backgroundTaskFactory.StartNew(() => action.DynamicInvoke(message));
                        _baseQueue.Add(target, bgTask);
                    }
                    break;

                case TaskType.Periodic:
                    var periodicTask = new PeriodicTask(() => _eventAggregator.Publish(statusEvent), _budget);
                    periodicTask.Start(_preemptiveScheduler);
                    break;

                case TaskType.Sporadic:
                    // one Periodic to run all sporadics
                    var sporadicTask = new SporadicTask(() => action.DynamicInvoke(message), _budget);
                    sporadicTask.Start(_preemptiveScheduler);
                    break;
                case TaskType.LongRunning:
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
