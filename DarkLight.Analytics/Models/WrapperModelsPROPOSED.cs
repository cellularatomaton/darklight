using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using DarkLight.Utilities;
using TradeLink.API;
using TradeLink.Common;

namespace DarkLight.Analytics.Models
{

    public class DarkLightSimBroker : Actor, IDarkLightBroker
    {
        #region Private Members

        Broker _broker;
        BackgroundWorker _backgroundWorker;
        HistSim _historicalSimulator;
        PlayTo _playTo;
        List<string> _historicalDataFiles = new List<string>();
        string _nowTime = "0";
        AutoResetEvent _are;

        #endregion

        #region Public Members

        #endregion

        #region Constructors

        public DarkLightSimBroker(IHub hub, BrokerConfigurationModel config)
            : base(hub)
        {
            //Plug into Tradelink
            _broker = new Broker();
            _broker.UseBidAskFills = config.SimUseBidAskFills;
            _broker.GotFill += PublishFill;
            _broker.GotOrder += PublishOrderAck;
            _broker.GotOrderCancel += PublishOrderCancelAck;

            loadTickData(config.TickFiles);
            _historicalSimulator = new MultiSimImpl(_historicalDataFiles.ToArray());
            _historicalSimulator.GotTick += PublishTick;

            //Plug into Hub
            if (hub.EventDict[EventType.Basket] != null)
                hub.EventDict[EventType.Basket] += OnBasket;
            else
                hub.EventDict[EventType.Basket] = OnBasket;

            if (hub.EventDict[EventType.CancelOrder] != null)
                hub.EventDict[EventType.CancelOrder] += OnCancelOrder;
            else
                hub.EventDict[EventType.CancelOrder] = OnCancelOrder;

            if (hub.EventDict[EventType.Order] != null)
                hub.EventDict[EventType.Order] += OnOrder;
            else
                hub.EventDict[EventType.Order] = OnOrder;

            //Set up tick player
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += play;
            _backgroundWorker.WorkerReportsProgress = false;
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.RunWorkerCompleted += playComplete;

            _playTo = config.PlayToValue;
            //_sessionRunning = config.SessionRunning;
        }

        #endregion

        #region Private Methods

        private void play(object sender, DoWorkEventArgs e)
        {
            PlayTo type = (PlayTo)e.Argument;
            if (e.Cancel) return;
            int time = (int)(_historicalSimulator.NextTickTime % 100000);
            long date = (_historicalSimulator.NextTickTime / 100000) * 100000;
            int t = (int)type;
            long val = 0;
            _playTo = type;
            switch (type)
            {
                case PlayTo.End:
                    val = MultiSimImpl.ENDSIM;
                    break;
                case PlayTo.FiveMin:
                case PlayTo.OneMin:
                case PlayTo.TenMin:
                case PlayTo.HalfHour:
                    val = date + Util.FTADD(time, (t / 10) * 60);
                    break;
                case PlayTo.Hour:
                    val = date + Util.FTADD(time, (t / 1000) * 3600);
                    break;
                case PlayTo.TwoHour:
                    val = date + Util.FTADD(time, 2 * 60 * 60);
                    break;
                case PlayTo.FourHour:
                    val = date + Util.FTADD(time, 4 * 60 * 60);
                    break;
                case PlayTo.OneSec:
                case PlayTo.ThirtySec:
                    val = date + Util.FTADD(time, t);
                    break;
            }

            _historicalSimulator.PlayTo(val);
        }

        private void playComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                PublishMessage(e.Error.Message + e.Error.StackTrace);
                PublishMessage("Terminated because of an Exception.  See messages.");
            }
            else if (e.Cancelled)
                PublishMessage("Canceled play.");
            else
            {
                string playToString = _playTo.ToString();
                PublishMessage("Reached next " + playToString + " at time " + KadTime);
            }

            

            PublishBacktestComplete();
        }

        private void loadTickData(List<string> paths)
        {
            _historicalDataFiles.Clear();
            foreach (var path in paths)
            {
                string f = path;
                if (isTIK(f))
                {
                    if (System.IO.File.Exists(f))
                    {
                        if (SecurityImpl.SecurityFromFileName(f).isValid)
                        {
                            _historicalDataFiles.Add(f);
                        }
                    }
                }
            }
        }

        private bool isTIK(string path)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(path, TikConst.EXT,
                                                                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        private string KadTime
        {
            get { return _nowTime != "" ? _nowTime : "(none)"; }
        }

        #endregion

        #region Public Methods

        public override void Start()
        {
            var de = new DarkLightEventArgs(EventType.ReportMessage);
            de.ReportMessageType = ReportMessageType.BrokerInfo;
            de.BrokerInfo = new BrokerInfo();
            de.BrokerInfo.SimPrettyTickFiles = ReportModel2.PrettyTickDataFiles(_historicalDataFiles);
            _publish(de);

            string message = "Playing next " + _playTo.ToString().Replace("Play +", string.Empty) + "...";  //verify "Play +" vs _playToString
            PublishMessage(message);

            _backgroundWorker.RunWorkerAsync(_playTo);
        }

        public void Shutdown()
        {
        }

        //TODO: remove
        public HistSim HistSimulator
        {
            get { return _historicalSimulator; }
            set { _historicalSimulator = value; }
        }

        #endregion

        #region Subscription Methods

        protected override void OnBasket(DarkLightEventArgs de)
        {
        }

        protected override void OnCancelOrder(DarkLightEventArgs de)
        {
            var orderid = de.Long;
            _broker.CancelOrder(orderid);
        }

        protected override void OnOrder(DarkLightEventArgs de)
        {
            _broker.SendOrderStatus(de.Order);
        }

        #endregion

        #region Publication Methods

        public void PublishBacktestComplete()
        {
            var de = new DarkLightEventArgs(EventType.ReportMessage);
            de.ReportMessageType = ReportMessageType.BacktestComplete;
            _publish(de);
        }

        public void PublishFill(Trade trade)
        {
            var de = new DarkLightEventArgs(EventType.Fill);
            de.Trade = trade;

            _publish(de);
        }

        public void PublishOrderAck(Order order)
        {
            var de = new DarkLightEventArgs(EventType.OrderAck);
            de.Order = order;

            _publish(de);
        }

        public void PublishOrderCancelAck(string s, bool b, long l)
        {
            var de = new DarkLightEventArgs(EventType.CancelOrderAck);
            de.String = s;
            de.Boolean = b;
            de.Long = l;

            _publish(de);
        }

        public void PublishTick(Tick tick)
        {
            _broker.Execute(tick);

            var de = new DarkLightEventArgs(EventType.Tick);
            de.Tick = tick;
            double numberTicksProcessed = Convert.ToDouble(_historicalSimulator.TicksProcessed);  //TODO: refactor 
            double totalNumberTicks = Convert.ToDouble(_historicalSimulator.TicksPresent);
            int percentComplete = Convert.ToInt32(Math.Round((numberTicksProcessed / totalNumberTicks) * 100));
            de.Integer = percentComplete;

            _publish(de);
        }

        #endregion
    }

    public class DarkLightResponse : Actor, IDarkLightResponse
    {
        #region Private Members

        Response _response;
        int _date = 0;
        int _time = 0;

        #endregion

        #region Public Members

        public override void Start()
        {
            var de = new DarkLightEventArgs(EventType.ReportMessage);
            de.ReportMessageType = ReportMessageType.ResponseInfo;
            de.ResponseInfo = new ResponseInfo();
            de.ResponseInfo.ResponseName = _response.Name;
            de.ResponseInfo.Indicators = getIndicators();
            _publish(de);
        }

        public Response ResponseInstance
        {
            get { return _response; }
            set { _response = value; }
        }

        public string[] Indicators
        {
            get { return _response.Indicators; }
        }

        #endregion

        #region Constructors

        public DarkLightResponse(IHub hub, Response response)
            : base(hub)
        {
            _type = ActorType.Response;

            //Plug into Tradelink
            _response = response;
            _response.SendBasketEvent += PublishBasket;
            _response.SendCancelEvent += PublishCancelOrder;
            _response.SendChartLabelEvent += PublishChartLabel;
            _response.SendIndicatorsEvent += PublishIndicators;
            _response.SendDebugEvent += PublishMessage;
            _response.SendOrderEvent += PublishOrder;

            //Plug into ZeroMQ
            if (hub.EventDict[EventType.CancelOrderAck] != null)
                hub.EventDict[EventType.CancelOrderAck] += OnCancelOrderAck;
            else
                hub.EventDict[EventType.CancelOrderAck] = OnCancelOrderAck;

            if (hub.EventDict[EventType.Fill] != null)
                hub.EventDict[EventType.Fill] += OnFill;
            else
                hub.EventDict[EventType.Fill] = OnFill;

            if (hub.EventDict[EventType.Message] != null)
                hub.EventDict[EventType.Message] += OnMessage;
            else
                hub.EventDict[EventType.Message] = OnMessage;

            if (hub.EventDict[EventType.OrderAck] != null)
                hub.EventDict[EventType.OrderAck] += OnOrderAck;
            else
                hub.EventDict[EventType.OrderAck] += OnOrderAck;

            //if (hub.EventDict[EventType.ServiceTransition] != null)
            //    hub.EventDict[EventType.ServiceTransition] += OnServiceTransition;
            //else
            //    hub.EventDict[EventType.ServiceTransition] += OnServiceTransition;

            if (hub.EventDict[EventType.Tick] != null)
                hub.EventDict[EventType.Tick] += OnTick;
            else
                hub.EventDict[EventType.Tick] = OnTick;
        }

        #endregion

        #region General Methods

        public void Reset()
        {
            _response.Reset();
        }

        #endregion

        #region Subscription Methods

        protected override void OnFill(DarkLightEventArgs de)
        {
            _response.GotFill(de.Trade);
        }

        protected override void OnOrderAck(DarkLightEventArgs de)
        {
            _response.GotOrder(de.Order);
        }

        protected override void OnServiceTransition(DarkLightEventArgs de)
        {

        }

        protected override void OnTick(DarkLightEventArgs de)
        {
            _date = de.Tick.date;
            _time = de.Tick.time;

            _response.GotTick(de.Tick);
        }

        #endregion

        #region Publication Methods

        public void PublishBasket(Basket basket, int index)
        {

        }

        public void PublishCancelOrder(long id, int index)
        {

        }

        public void PublishChartLabel(decimal d, int index, string s, System.Drawing.Color color)
        {
            var de = new DarkLightEventArgs(EventType.ChartLabel);
            de.Decimal = d;
            de.Integer = index;
            de.String = s;
            de.Color = color;

            _publish(de);
        }

        public void PublishIndicators(int index, string indicator)
        {
            var de = new DarkLightEventArgs(EventType.Indicator);
            de.Integer = index;
            de.String = indicator;

            _publish(de);
        }

        public void PublishOrder(Order order, int index)
        {
            if (order.time == 0)
            {
                order.date = _date;
                order.time = _time;
            }

            var de = new DarkLightEventArgs(EventType.Order);
            de.Order = order;
            de.Integer = index;

            _publish(de);
        }

        #endregion

        #region Private Methods

        string[] getIndicators()
        {
            string[] indicators = null;

            if (!((_response == null) || (_response.Indicators == null) || (_response.Indicators.Length == 0)))
                indicators = _response.Indicators;

            return indicators;
        }

        #endregion
    }

    public class Actor : IActor
    {
        protected ActorType _type;
        public ActorType Type { get { return _type; } }

        protected Action<DarkLightEventArgs> _publish;

        #region Constructors

        public Actor(IHub hub)
        {
            _publish = hub.Publish;

            /*
             *  RESPECTIVE EVENTS FROM BELOW SUBSCRIBED TO IN DERIVED CLASSES
             *  
                hub.EventDict[EventType.Basket] += OnBasket;
                hub.EventDict[EventType.CancelOrder] += OnCancelOrder;
                hub.EventDict[EventType.CancelOrderAck] += OnCancelOrderAck;
                hub.EventDict[EventType.ChartLabel] += OnChartLabel;
                hub.EventDict[EventType.Fill] += OnFill;
                hub.EventDict[EventType.Indicator] += OnIndicator;
                hub.EventDict[EventType.Message] += OnMessage;
                hub.EventDict[EventType.Order] += OnOrder;
                hub.EventDict[EventType.OrderAck] += OnOrderAck;
                hub.EventDict[EventType.Plot] += OnPlot;
                hub.EventDict[EventType.Position] += OnPosition;
                hub.EventDict[EventType.ReportRegister] += OnReportRegister;
                hub.EventDict[EventType.ServiceTransition] += OnServiceTransition;
                hub.EventDict[EventType.Status] += OnStatus;
                hub.EventDict[EventType.Tick] += OnTick;
            */
        }

        #endregion

        #region Public Methods

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
        }

        #endregion

        #region Protected Methods: Packet Handlers

        protected virtual void OnBasket(DarkLightEventArgs de)
        {
            //Not implemented      
        }

        protected virtual void OnCancelOrder(DarkLightEventArgs de)
        {
            //Not implemented                
        }

        protected virtual void OnCancelOrderAck(DarkLightEventArgs de)
        {
            //Not implemented               
        }

        protected virtual void OnChartLabel(DarkLightEventArgs de)
        {
            //publishers:  Response
            //subscribers: Report               
        }

        protected virtual void OnFill(DarkLightEventArgs de)
        {
            //publishers:  Broker
            //subscribers: Response, Report  
        }

        protected virtual void OnIndicator(DarkLightEventArgs de)
        {
            //publishers:  Response
            //subscribers: Report  
        }

        protected virtual void OnMessage(DarkLightEventArgs de)
        {
            //publishers:  Broker, Response
            //subscribers: Report  
        }

        protected virtual void OnOrder(DarkLightEventArgs de)
        {
            //publishers:  Response
            //subscribers: Broker, Report  
        }

        protected virtual void OnOrderAck(DarkLightEventArgs de)
        {
            //publishers:  Broker
            //subscribers: Response, Report   
        }

        protected virtual void OnPlot(DarkLightEventArgs de)
        {
            //publishers:  Response
            //subscribers: Report   
        }

        protected virtual void OnPosition(DarkLightEventArgs de)
        {
            //publishers:  Response
            //subscribers: Report   
        }

        protected virtual void OnServiceTransition(DarkLightEventArgs de)
        {
            //publishers:  Broker, Response, Report 
            //subscribers: Broker, Response, Report 
        }

        protected virtual void OnStatus(DarkLightEventArgs de)
        {
            //publishers:  Broker, Response, Report 
            //subscribers: Report 
        }

        protected virtual void OnReportMessage(DarkLightEventArgs de)
        {
            //publishers:  Broker, Response
            //subscribers: Report 
        }

        protected virtual void OnTick(DarkLightEventArgs de)
        {
            //publishers:  Broker
            //subscribers: Response, Report 
        }

        #endregion

        #region Protected Methods: Common Publishers

        protected virtual void PublishMessage(string msg)
        {
            var de = new DarkLightEventArgs(EventType.Message);
            de.String = msg;

            _publish(de);
        }

        #endregion

    }
    
}
