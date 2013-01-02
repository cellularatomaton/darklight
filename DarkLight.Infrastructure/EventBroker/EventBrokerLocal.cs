using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DarkLight.Framework.Data.Common;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Adapters;
using DarkLight.Framework.Interfaces.CEP;
using DarkLight.Framework.Interfaces.Services;
using com.espertech.esper.client;
using EventType = DarkLight.Framework.Enums.EventType;

namespace DarkLight.Infrastructure.EventBroker
{
    public class EventBrokerLocal : IEventBroker
    {
        private DarkLightEventAggregator _eventAggregator;
        EPServiceProvider _epService;
        Dictionary<System.Type, EPStatement> statementDict;
        Dictionary<string, EPStatement> customStatementDict;

        public EventBrokerLocal()
        {
            _eventAggregator = new DarkLightEventAggregator();

            Configuration configuration = new Configuration();
            configuration.AddEventType("BacktestRequestEvent", typeof(BacktestRequestEvent).FullName);
            configuration.AddEventType("LinkedNavigationEvent", typeof(LinkedNavigationEvent).FullName);
            configuration.AddEventType("StatusEvent", typeof(StatusEvent).FullName);
            configuration.AddEventType("TradeEvent", typeof(TradeEvent).FullName);
            configuration.EngineDefaults.EventMetaConfig.ClassPropertyResolutionStyle = PropertyResolutionStyle.CASE_INSENSITIVE;
            _epService = EPServiceProviderManager.GetProvider("EventBrokerLocal", configuration);

            statementDict = new Dictionary<System.Type, EPStatement>();
            statementDict.Add(typeof(BacktestRequestEvent), _epService.EPAdministrator.CreateEPL("select * from BacktestRequestEvent"));
            statementDict.Add(typeof(LinkedNavigationEvent), _epService.EPAdministrator.CreateEPL("select * from LinkedNavigationEvent"));
            statementDict.Add(typeof(StatusEvent), _epService.EPAdministrator.CreateEPL("select * from StatusEvent"));
            statementDict.Add(typeof(TradeEvent), _epService.EPAdministrator.CreateEPL("select * from TradeEvent"));

            statementDict[typeof(BacktestRequestEvent)].Events += _eventAggregator.Update;
            statementDict[typeof(LinkedNavigationEvent)].Events += _eventAggregator.Update;
            statementDict[typeof(StatusEvent)].Events += _eventAggregator.Update;
            statementDict[typeof(TradeEvent)].Events += _eventAggregator.Update;
            
            //TODO: for queries typed into GUI
            customStatementDict = new Dictionary<string, EPStatement>();
        }

        public void Publish(DarkLightEvent darkLightEvent)
        {
            _epService.EPRuntime.SendEvent(darkLightEvent);
        }

        public void Subscribe(object instance)
        {
            _eventAggregator.Subscribe(instance);
        }

        public void Subscribe(object instance, string message)
        {
            var methodInfo = instance.GetType().GetMethod("Update");
            var updateMethod = (UpdateEventHandler)Delegate.CreateDelegate(typeof(UpdateEventHandler), instance, methodInfo);
            var statement = _epService.EPAdministrator.CreateEPL(message);

            customStatementDict[message] = statement;
            customStatementDict[message].Events += updateMethod;
        } 
        
    }

    //EventAggregator & ExtensionMethods lifted directly from Caliburn Micro source
    //Only modification is the added Update method to hook into NEsper engine, could be moved to EventBroker itself
    public class DarkLightEventAggregator
    {
        readonly List<Handler> handlers = new List<Handler>();

        public static Action<System.Action> DefaultPublicationThreadMarshaller = action => action();

        public static Action<object, object> HandlerResultProcessing = (target, result) => { };

        public DarkLightEventAggregator()
        {
            PublicationThreadMarshaller = DefaultPublicationThreadMarshaller;
        }

        public Action<System.Action> PublicationThreadMarshaller { get; set; }

        public virtual void Subscribe(object instance)
        {
            lock (handlers)
            {
                if (handlers.Any(x => x.Matches(instance)))
                {
                    return;
                }

                handlers.Add(new Handler(instance));
            }
        }

        public virtual void Unsubscribe(object instance)
        {
            lock (handlers)
            {
                var found = handlers.FirstOrDefault(x => x.Matches(instance));

                if (found != null)
                {
                    handlers.Remove(found);
                }
            }
        }

        //hook for NEsper engine
        public void Update(object sender, UpdateEventArgs e)
        {
            Publish(e.NewEvents[0].Underlying, PublicationThreadMarshaller);
        }

        public virtual void Publish(object message)
        {
            Publish(message, PublicationThreadMarshaller);
        }

        public virtual void Publish(object message, Action<System.Action> marshal)
        {
            Handler[] toNotify;
            lock (handlers)
            {
                toNotify = handlers.ToArray();
            }

            marshal(() =>
            {
                var messageType = message.GetType();

                var dead = toNotify
                    .Where(handler => !handler.Handle(messageType, message))
                    .ToList();

                if (dead.Any())
                {
                    lock (handlers)
                    {
                        dead.Apply(x => handlers.Remove(x));
                    }
                }
            });
        }
     
        protected class Handler
        {
            readonly WeakReference reference;
            readonly Dictionary<Type, MethodInfo> supportedHandlers = new Dictionary<Type, MethodInfo>();

            public Handler(object handler)
            {
                reference = new WeakReference(handler);

                var interfaces = handler.GetType().GetInterfaces()
                    .Where(x => typeof(IHandle).IsAssignableFrom(x) && x.IsGenericType);

                foreach (var @interface in interfaces)
                {
                    var type = @interface.GetGenericArguments()[0];
                    var method = @interface.GetMethod("Handle");
                    supportedHandlers[type] = method;
                }
            }

            public bool Matches(object instance)
            {
                return reference.Target == instance;
            }

            public bool Handle(Type messageType, object message)
            {
                var target = reference.Target;
                if (target == null)
                {
                    return false;
                }

                foreach (var pair in supportedHandlers)
                {
                    if (pair.Key.IsAssignableFrom(messageType))
                    {
                        var result = pair.Value.Invoke(target, new[] { message });
                        if (result != null)
                        {
                            HandlerResultProcessing(target, result);
                        }
                        return true;
                    }
                }

                return true;
            }
        }
    }

    public static class ExtensionMethods
    {

        public static void Apply<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }
    }
}
