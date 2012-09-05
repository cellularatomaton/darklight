using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using DarkLight.Utilities;
using TradeLink.API;

namespace DarkLight.Analytics.Models
{

    public class SessionModel : IDisposable
    {
        #region Private Members

        IHub _hub;
        List<IDarkLightResponse> _responseList;
        IDarkLightBroker _broker;
        IReporter _reporter;
        //IRiskModel _riskModel;
        //IPortfolioModel _portfolioModel;
        ManualResetEventSlim _mre = new ManualResetEventSlim(false);

        #endregion

        #region Public Members

        public Response ResponseInstance
        {
            get { return _responseList[0].ResponseInstance; }
            set
            {
                if (value != _responseList[0])
                {
                    _responseList[0].ResponseInstance = value;
                    NotifyPropertyChanged("ResponseInstance");
                }
            }
        }

        public IReporter Reporter { get { return _reporter; } }

        public void Run()
        {
            Start();
            _mre.Wait();
        }

        public void Start()
        {
            _hub.Start();
            _responseList[0].Start();
            _broker.Start();
        }

        public void Subscribe(byte[] eventType, Action<DarkLightEventArgs> handler)
        {
            if (_hub.EventDict[eventType] != null)
                _hub.EventDict[eventType] += handler;
            else
                _hub.EventDict[eventType] = handler;
        }

        public void Unsubscribe(byte[] eventType, Action<DarkLightEventArgs> handler)
        {
            _hub.EventDict[eventType] -= handler;
        }

        public void Shutdown()
        {
        }

        #endregion

        #region Constructors

        public SessionModel(HubConfigurationModel hubConfig, BrokerConfigurationModel brokerConfig,
                            ResponseConfigurationModel responseConfig, ReportConfigurationModel reportConfig)
        {
            _hub = DarkLightFactory.GetHub(hubConfig);
            _broker = DarkLightFactory.GetBroker(_hub, brokerConfig);
            _responseList = DarkLightFactory.GetResponseList(_hub, responseConfig);
            _reporter = DarkLightFactory.GetReporter(_hub, reportConfig);
            //_riskMgr = DarkLightFactory.GetRiskManager(_hub, reportConfig);
            //_portfolioMgr = DarkLightFactory.GetPortfolioManager(_hub, reportConfig);

            //_hub.EventDict[EventType.ReportMessage]
        }

        public void Reset()
        { }

        #endregion

        #region Implementation of IDisposable

        void DisposeAll()
        {
            //_eventBroker.Dispose();            
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
        bool disposed = false;

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
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
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
        ~SessionModel()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }

    public class DarkLightFactory
    {
        public static IHub GetHub(HubConfigurationModel config)
        {
            switch (config.Type)
            {
                case HubType.Local:
                    return new LocalHub();
                case HubType.Zero:
                    return new ZeroHub();
                case HubType.Mixed:
                    return new MixedHub();
                default:
                    return new Hub();
            }
        }

        public static IDarkLightBroker GetBroker(IHub hub, BrokerConfigurationModel config)
        {
            switch (config.Type)
            {
                case BrokerType.Sim:
                    return new DarkLightSimBroker(hub, config);
                case BrokerType.Live:
                    return new DarkLightSimBroker(hub, config);
                default:
                    return new DarkLightSimBroker(hub, config);
            }
        }

        public static List<IDarkLightResponse> GetResponseList(IHub hub, ResponseConfigurationModel config)
        {
            var dlResponseList = new List<IDarkLightResponse>();

            foreach (var response in config.ResponseList)
            {
                IDarkLightResponse dlResponse = new DarkLightResponse(hub, config);
                dlResponseList.Add(dlResponse);
            }

            return dlResponseList;
        }

        public static IReporter GetReporter(IHub hub, ReportConfigurationModel config)
        {
            switch (config.Type)
            {
                case ReportType.Batch:
                    return new BatchReportModel2(hub, config);
                case ReportType.Streaming:
                    return new BatchReportModel2(hub, config);  //TODO: make streaming model with _isBatch = false
                default:
                    return new BatchReportModel2(hub, config);
            }
        }
    }

}
