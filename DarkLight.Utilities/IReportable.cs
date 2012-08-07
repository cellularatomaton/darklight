using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using System.Windows.Threading;

namespace DarkLight.Utilities
{
    public interface IReportable
    {
        event Action<string> StatusUpdate;
        event Action<string> MessageUpdate;
        event Action<Tick> GotTick;
        event Action<Trade> GotFill;
        event Action<Order> GotOrder;
        event Action<Position> GotPosition;
        event Action<TimePlot> GotPlot;
        event Action<string[]> GotIndicators;
    }

    public enum DispatchableType
    {
        Status,
        Message,
        Tick,
        Fill,
        Order,
        Position,
        Plot,
        Indicator,
    }

    public class ReportModel : IDisposable
    {
        private IReportable _engine;
        private Dictionary<DispatchableType, Dispatcher> _dispatchingMap = new Dictionary<DispatchableType, Dispatcher>(); 

        public ReportModel(IReportable engine)
        {
            _engine = engine;
            _engine.StatusUpdate += engine_StatusUpdate;
            _engine.MessageUpdate += engine_MessageUpdate;
            _engine.GotTick += engine_GotTick;
            _engine.GotFill += engine_GotFill;
            _engine.GotOrder += engine_GotOrder;
            _engine.GotPosition += engine_GotPosition;
            _engine.GotPlot += engine_GotPlot;
            _engine.GotIndicators += engine_GotIndicators;

            // Default each dispatchable type to the current thread's dispatcher.  Any controls which register a dispatcher will change this.
            var dispatchValues = Enum.GetValues(typeof(DispatchableType)).Cast<DispatchableType>();
            foreach (var _value in dispatchValues)
            {
                _dispatchingMap[_value] = Dispatcher.CurrentDispatcher;
            }
        }

        // Register a dispatcher from the control thread.
        public void RegisterDispatcher(DispatchableType dispatchableType, Dispatcher dispatcher)
        {
            _dispatchingMap[dispatchableType] = dispatcher;
        }

        void engine_GotIndicators(string[] obj)
        {
            _dispatchingMap[DispatchableType.Indicator].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        void engine_GotPlot(TimePlot obj)
        {
            _dispatchingMap[DispatchableType.Plot].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        void engine_GotPosition(Position obj)
        {
            _dispatchingMap[DispatchableType.Position].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        void engine_GotOrder(Order obj)
        {
            _dispatchingMap[DispatchableType.Order].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        void engine_GotFill(Trade obj)
        {
            _dispatchingMap[DispatchableType.Fill].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        void engine_GotTick(Tick obj)
        {
            _dispatchingMap[DispatchableType.Tick].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        void engine_StatusUpdate(string obj)
        {
            _dispatchingMap[DispatchableType.Status].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        void engine_MessageUpdate(string obj)
        {
            _dispatchingMap[DispatchableType.Message].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        #region Implementation of IDisposable

        private void DisposeAll()
        {
            _engine.StatusUpdate -= engine_StatusUpdate;
            _engine.MessageUpdate -= engine_MessageUpdate;
            _engine.GotTick -= engine_GotTick;
            _engine.GotFill -= engine_GotFill;
            _engine.GotOrder -= engine_GotOrder;
            _engine.GotPosition -= engine_GotPosition;
            _engine.GotPlot -= engine_GotPlot;
            _engine.GotIndicators -= engine_GotIndicators;
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Track whether Dispose has been called.
        private bool disposed = false;

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if(!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if(disposing)
                {
                    // Dispose managed resources.
                    DisposeAll();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~ReportModel()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion
    }

    /// <summary>
    /// The batch report model only updates its observable collections when specified.
    /// </summary>
    public class BatchReportModel : ReportModel
    {
        public BatchReportModel(IReportable engine) : base(engine)
        {
        }
    }

    /// <summary>
    /// The streaming report model updates its observable collections continuously.
    /// </summary>
    public class StreamingReportModel : ReportModel
    {
        public StreamingReportModel(IReportable engine)
            : base(engine)
        {
        }
    }
}
