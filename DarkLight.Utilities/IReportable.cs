using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using DarkLight.Analytics.Models;
using TradeLink.API;
using TradeLink.AppKit;
using TradeLink.Common;
using System.Windows.Threading;
using TradeLink.Common;

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
        event Action<EngineInfo> EngineReset;
        event Action<RunWorkerCompletedEventArgs> EngineComplete;
    }

    public class ReportModel : INotifyPropertyChanged, IDisposable
    {
        #region Protected Members

        protected IReportable _engine;
        protected ActivityModel _activityModel;
        protected Dictionary<DispatchableType, Dispatcher> _dispatchingMap = new Dictionary<DispatchableType, Dispatcher>();
        protected EngineInfo _engineInfo = new EngineInfo();

        #endregion

        #region Constructors

        public ReportModel(IReportable engine, ActivityModel activityModel)
        {
            _activityModel = activityModel;

            _engine = engine;
            _engine.StatusUpdate += engine_StatusUpdate;
            _engine.MessageUpdate += engine_MessageUpdate;
            _engine.GotTick += engine_GotTick;
            _engine.GotFill += engine_GotFill;
            _engine.GotOrder += engine_GotOrder;
            _engine.GotPosition += engine_GotPosition;
            _engine.GotPlot += engine_GotPlot;
            _engine.GotIndicators += engine_GotIndicators;
            _engine.EngineReset += engine_Reset;
            _engine.EngineComplete += engine_Complete;

            // Default each dispatchable type to the current thread's dispatcher.  Any controls which register a dispatcher will change this.
            var dispatchValues = Enum.GetValues(typeof(DispatchableType)).Cast<DispatchableType>();
            foreach (var _value in dispatchValues)
            {
                _dispatchingMap[_value] = Dispatcher.CurrentDispatcher;
            }
        }

        #endregion

        #region Public Methods

        // Register a dispatcher from the control thread.
        public void RegisterDispatcher(DispatchableType dispatchableType, Dispatcher dispatcher)
        {
            _dispatchingMap[dispatchableType] = dispatcher;
        }

        #endregion

        #region Protected Methods

        protected virtual void engine_GotIndicators(string[] obj)
        {
            _dispatchingMap[DispatchableType.Indicator].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        protected virtual void engine_GotPlot(TimePlot obj)
        {
            _dispatchingMap[DispatchableType.Plot].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        protected virtual void engine_GotPosition(Position obj)
        {
            _dispatchingMap[DispatchableType.Position].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        protected virtual void engine_GotOrder(Order obj)
        {
            _dispatchingMap[DispatchableType.Order].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        protected virtual void engine_GotFill(Trade obj)
        {
            _dispatchingMap[DispatchableType.Fill].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        protected virtual void engine_GotTick(Tick obj)
        {
            _dispatchingMap[DispatchableType.Tick].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        protected virtual void engine_StatusUpdate(string obj)
        {
            _dispatchingMap[DispatchableType.Status].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        protected virtual void engine_MessageUpdate(string obj)
        {
            _dispatchingMap[DispatchableType.Message].Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Do you thang...
            }));
        }

        protected virtual void engine_Reset(EngineInfo obj)
        {
            //TODO: create general arguments obj that can be used by backtester/optimizer/live trader
        }

        protected virtual void engine_Complete(RunWorkerCompletedEventArgs obj)
        {
            //BackgroundWorker complete handler which fired this event should have run on UI thread (standard WPF implementation), 
            //this method may or may not run on UI thread as well, implement Dispatcher with CheckAccess to avoid unnecessary Dispatcher.Invokes (perhaps make this an extension method)
        }

        #endregion

        #region Implementation of IDisposable

        void DisposeAll()
        {
            _engine.StatusUpdate -= engine_StatusUpdate;
            _engine.MessageUpdate -= engine_MessageUpdate;
            _engine.GotTick -= engine_GotTick;
            _engine.GotFill -= engine_GotFill;
            _engine.GotOrder -= engine_GotOrder;
            _engine.GotPosition -= engine_GotPosition;
            _engine.GotPlot -= engine_GotPlot;
            _engine.GotIndicators -= engine_GotIndicators;
            _engine.EngineReset -= engine_Reset;
            _engine.EngineComplete -= engine_Complete;
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
        ~ReportModel()
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

    public class BatchReportModel : ReportModel
    {
        #region Public Members

        //private DataTable _completedTickTable;
        private ObservableCollection<DataGridTick> _completedTickTable;
        public ObservableCollection<DataGridTick> TickTable
        {
            get { return _completedTickTable; }
            set
            {
                if (value != _completedTickTable)
                {
                    _completedTickTable = value;
                    NotifyPropertyChanged("TickTable");
                }
            }
        }

        private DataTable _completedIndicatorTable;
        public DataTable IndicatorTable
        {
            get { return _completedIndicatorTable; }
            set
            {
                if (value != _completedIndicatorTable)
                {
                    _completedIndicatorTable = value;
                    NotifyPropertyChanged("IndicatorTable");
                }
            }
        }

        private ObservableCollection<DataGridPosition> _completedPositionTable;
        public ObservableCollection<DataGridPosition> PositionTable
        {
            get { return _completedPositionTable; }
            set
            {
                if (value != _completedPositionTable)
                {
                    _completedPositionTable = value;
                    NotifyPropertyChanged("PositionTable");
                }
            }
        }

        private ObservableCollection<DataGridOrder> _completedOrderTable;
        public ObservableCollection<DataGridOrder> OrderTable
        {
            get { return _completedOrderTable; }
            set
            {
                if (value != _completedOrderTable)
                {
                    _completedOrderTable = value;
                    NotifyPropertyChanged("OrderTable");
                }
            }
        }

        private ObservableCollection<DataGridFill> _completedFillTable;
        public ObservableCollection<DataGridFill> FillTable
        {
            get { return _completedFillTable; }
            set
            {
                if (value != _completedFillTable)
                {
                    _completedFillTable = value;
                    NotifyPropertyChanged("FillTable");
                }
            }
        }

        DataTable _completedResultsTable;
        public DataTable ResultsTable
        {
            get { return _completedResultsTable; }
            set
            {
                if (value != _completedResultsTable)
                {
                    _completedResultsTable = value;
                    NotifyPropertyChanged("ResultsTable");
                }
            }
        }

        Dictionary<string, TimePlot> _plotMap = new Dictionary<string, TimePlot>();
        private ObservableCollection<TimePlot> _completedPlots;
        public ObservableCollection<TimePlot> Plots
        {
            get { return _completedPlots; }
            set
            {
                if (value != _completedPlots)
                {
                    _completedPlots = value;
                    NotifyPropertyChanged("Plots");
                }
            }
        }

        #endregion

        #region Private Members

        Dictionary<string, PositionImpl> _positionList = new Dictionary<string, PositionImpl>();
        List<Trade> _tradeList = new List<Trade>();
        ResultsModel _resultsModel = new ResultsModel();
        StringBuilder _messageBuilder = new StringBuilder();
        Action _updatePlots;

        DataTable _indicatorTable = new DataTable();// = new DataTable("indicatorTable");
        ObservableCollection<DataGridTick> _tickCollection = new ObservableCollection<DataGridTick>();
        ObservableCollection<DataGridPosition> _positionCollection = new ObservableCollection<DataGridPosition>();
        ObservableCollection<DataGridOrder> _orderCollection = new ObservableCollection<DataGridOrder>();
        ObservableCollection<DataGridFill> _fillCollection = new ObservableCollection<DataGridFill>();

        bool _missingIndicatorWarn = true;
        int _decimalPrecision = 2;
        string _decimalPrecisionString = "N2";
        string _nowTime = "0";
        string _programName = "Incepto.Analytics";

        #endregion

        #region Constructors

        public BatchReportModel(IReportable engine, ActivityModel activityModel, Action updatePlots)
            : base(engine, activityModel)
        {
            _updatePlots = updatePlots;
            _decimalPrecisionString = "N" + _decimalPrecision;
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            _positionList.Clear();
            _tradeList.Clear();
            _messageBuilder = new StringBuilder();
            engine_MessageUpdate(Util.TLSIdentity());
            engine_MessageUpdate(RunTracker.CountNewGetPrettyRuns(_programName, Util.PROGRAM));
            initializeTables();
            initializeIndicators();
            _plotMap.Clear();
            _nowTime = "0";
        }

        #endregion

        #region Protected Methods

        //engine_GotPosition(Position obj) not yet implemented        

        protected override void engine_GotIndicators(string[] values)
        {
            try
            {
                _indicatorTable.Rows.Add(values);
            }
            catch (ArgumentException ex)
            {
                if (_missingIndicatorWarn && ex.Message.Contains("array is longer than the number of columns"))
                {
                    _missingIndicatorWarn = false;
                    engine_MessageUpdate("Your indicator names do not match the number of indicators you sent with sendindicators.");
                    engine_MessageUpdate("Check to make sure you do not have commas in your sendindicator values.");
                    engine_StatusUpdate("User error in specifying indicators.");
                }
            }
        }

        protected override void engine_GotOrder(Order order)
        {
            _orderCollection.Add(new DataGridOrder(order));
        }

        protected override void engine_GotFill(Trade t)
        {
            _tradeList.Add(t);
            PositionImpl mypos = new PositionImpl(t);
            decimal cpl = 0;
            decimal cpt = 0;
            if (!_positionList.TryGetValue(t.symbol, out mypos))
            {
                mypos = new PositionImpl(t);
                _positionList.Add(t.symbol, mypos);
            }
            else
            {
                cpt = Calc.ClosePT(mypos, t);
                cpl = mypos.Adjust(t);
                _positionList[t.symbol] = mypos;
            }

            _positionCollection.Add(new DataGridPosition(_nowTime, mypos, cpl, cpt, _decimalPrecisionString));
            _fillCollection.Add(new DataGridFill(t, _decimalPrecisionString));
        }

        protected override void engine_GotTick(Tick tick)
        {
            _nowTime = tick.time.ToString();
            _tickCollection.Add(new DataGridTick(tick, _decimalPrecisionString));

            double numberTicksProcessed = Convert.ToDouble(_engineInfo.HistoricalSimulator.TicksProcessed);  //TODO: refactor out HistSim
            double totalNumberTicks = Convert.ToDouble(_engineInfo.HistoricalSimulator.TicksPresent);
            int percentComplete = Convert.ToInt32(Math.Round((numberTicksProcessed / totalNumberTicks) * 100));
            _activityModel.PercentComplete = percentComplete;
        }

        protected override void engine_GotPlot(TimePlot plot)
        {
            var label = plot.Label;

            if (!_plotMap.ContainsKey(label))
            {
                _plotMap.Add(label, plot);
            }
            else
            {
                var point = plot.PlotPoints[0];
                _plotMap[label].PlotPoints.Add(point);
            }
        }

        protected override void engine_StatusUpdate(string status)
        {
            _activityModel.Status = status;
        }

        protected override void engine_MessageUpdate(string msg)
        {
            _messageBuilder.AppendFormat("{0}: {1}{2}", _nowTime, msg, Environment.NewLine);
        }

        protected override void engine_Reset(EngineInfo engineInfo)
        {
            _engineInfo = engineInfo;
            Reset();
        }

        protected override void engine_Complete(RunWorkerCompletedEventArgs e)
        {
            string[] r = _messageBuilder.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string l in r)
                _activityModel.Messages.Add(new ObservableMessage { Message = l });

            IndicatorTable = _indicatorTable;
            TickTable = _tickCollection;
            OrderTable = _orderCollection;
            FillTable = _fillCollection;
            PositionTable = _positionCollection;

            Plots = new ObservableCollection<TimePlot>(_plotMap.Values);
            _updatePlots();

            string responseName = _engineInfo.ResponseName;
            string prettyTickFiles = _engineInfo.PrettyTickFiles;
            ResultsTable = _resultsModel.RunResults(responseName + "." + prettyTickFiles, _tradeList);

            if (e.Error != null)
            {
                engine_MessageUpdate(e.Error.Message + e.Error.StackTrace);
                engine_StatusUpdate("Terminated because of an Exception.  See messages.");
            }
            else if (e.Cancelled)
                engine_StatusUpdate("Canceled play.");
            else
            {
                string playToString = _engineInfo.PlayToString;
                engine_StatusUpdate("Reached next " + playToString + " at time " + KadTime);
            }
        }

        #endregion

        #region Private Methods

        void initializeTables()
        {
            _positionCollection.Clear();
            _orderCollection.Clear();
            _fillCollection.Clear();
            _tickCollection.Clear();

            _resultsModel.Clear();

        }

        void initializeIndicators()
        {
            // clear existing indicators
            _indicatorTable.Clear();
            _indicatorTable.Columns.Clear();

            string[] indicators = _engineInfo.Indicators;

            if (indicators != null)
            {
                for (int i = 0; i < indicators.Length; i++)
                {
                    try
                    {
                        _indicatorTable.Columns.Add(indicators[i]);
                    }
                    catch (DuplicateNameException)
                    {
                        engine_MessageUpdate("You have duplicate column name: " + indicators[i] +
                                             " defined in your response.  Please remove this and try again.");
                    }
                }
            }
        }

        string KadTime
        {
            get
            {
                return _nowTime != "" ? _nowTime : "(none)";
            }
        }

        #endregion
    }

    public class OptimizationBatchReportModel : ReportModel
    {
        #region Public Members

        List<PlottableValue<DarkLightResults>> _optimizationResults = new List<PlottableValue<DarkLightResults>>();
        public List<PlottableValue<DarkLightResults>> OptimizationResults { get { return _optimizationResults; } }
        public double xValue { get; set; }

        #endregion

        #region Private Members
       
        List<Trade> _tradeList = new List<Trade>();
        ResultsModel _resultsModel = new ResultsModel();                
        Action _updatePlots;

        decimal _rfr = 0.00m;
        decimal _comm = .0012m;
        string _nowTime = "0";

        #endregion

        #region Constructors

        public OptimizationBatchReportModel(IReportable engine, ActivityModel activityModel, Action updatePlots)
            : base(engine, activityModel)
        {
            _updatePlots = updatePlots;
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            _tradeList.Clear();
            _resultsModel.Clear();
            //_optimizationResults.Clear();
            _nowTime = "0";
        }

        #endregion

        #region Protected Methods

        protected override void engine_GotPosition(Position obj)
        {

        }

        protected override void engine_GotIndicators(string[] values)
        {

        }

        protected override void engine_GotOrder(Order order)
        {
            
        }

        protected override void engine_GotFill(Trade t)
        {
            _tradeList.Add(t);
        }

        protected override void engine_GotTick(Tick tick)
        {
            _nowTime = tick.time.ToString();
            
            double numberTicksProcessed = Convert.ToDouble(_engineInfo.HistoricalSimulator.TicksProcessed);  //TODO: refactor out HistSim
            double totalNumberTicks = Convert.ToDouble(_engineInfo.HistoricalSimulator.TicksPresent);
            int percentComplete = Convert.ToInt32(Math.Round((numberTicksProcessed / totalNumberTicks) * 100));
            _activityModel.PercentComplete = percentComplete;
        }

        protected override void engine_GotPlot(TimePlot plot)
        {

        }

        protected override void engine_StatusUpdate(string status)
        {
            _activityModel.Status = status;
        }

        protected override void engine_MessageUpdate(string msg)
        {
            
        }

        protected override void engine_Reset(EngineInfo engineInfo)
        {
            _engineInfo = engineInfo;
            Reset();
        }

        protected override void engine_Complete(RunWorkerCompletedEventArgs e)
        {
            _resultsModel.Clear();
            var newResults = TradeResult.ResultsFromTradeList(_tradeList);
            _tradeList.Clear();
            var results = Results.FetchResults(newResults, _rfr, _comm, engine_MessageUpdate);
            var darkLightResults = new DarkLightResults(results);
            _optimizationResults.Add(new PlottableValue<DarkLightResults> { X = xValue, Value = darkLightResults });

            _updatePlots();

            if (e.Error != null)
            {
                engine_MessageUpdate(e.Error.Message + e.Error.StackTrace);
                engine_StatusUpdate("Terminated because of an Exception.  See messages.");
            }
            else if (e.Cancelled)
                engine_StatusUpdate("Canceled play.");
            else
            {
                string playToString = _engineInfo.PlayToString;
                engine_StatusUpdate("Reached next " + playToString + " at time " + KadTime);
            }
        }

        #endregion

        #region Private Methods

        string KadTime
        {
            get
            {
                return _nowTime != "" ? _nowTime : "(none)";
            }
        }

        #endregion
    }

    /// <summary>
    /// The streaming report model updates its observable collections continuously.
    /// </summary>
    public class StreamingReportModel : ReportModel
    {
        public StreamingReportModel(IReportable engine, ActivityModel activityModel)
            : base(engine, activityModel)
        {
        }
    }

    #region HELPER CLASSES

    public class EngineInfo
    {
        public EngineType Type;
        public string[] Indicators;
        public string ResponseName;
        public string PlayToString;
        public string PrettyTickFiles;
        //public double TotalNumberTicks;    //not initialized until after you start play, hence temporary hack below
        public HistSim HistoricalSimulator;  //TODO: get rid of this (used to get TotalNumberTicks)
    }

    public class DataGridTick
    {
        public string Time { get; set; }
        public string Sym { get; set; }
        public string Trade { get; set; }
        public string TSize { get; set; }
        public string Bid { get; set; }
        public string Ask { get; set; }
        public string BSize { get; set; }
        public string ASize { get; set; }
        public string TExch { get; set; }
        public string BidExch { get; set; }
        public string AskExch { get; set; }

        public DataGridTick(Tick tick, string decimalPrecisionString)
        {
            Time = tick.time.ToString();
            Trade = "";
            Bid = "";
            Ask = "";
            TSize = "";
            BSize = "";
            ASize = "";
            BidExch = "";
            AskExch = "";
            TExch = "";
            if (tick.isIndex)
            {
                Trade = tick.trade.ToString(decimalPrecisionString);
            }
            else if (tick.isTrade)
            {
                Trade = tick.trade.ToString(decimalPrecisionString);
                TSize = tick.size.ToString();
                TExch = tick.ex;
            }
            if (tick.hasBid)
            {
                BSize = tick.bs.ToString();
                BidExch = tick.be;
                Bid = tick.bid.ToString(decimalPrecisionString);
            }
            if (tick.hasAsk)
            {
                Ask = tick.ask.ToString(decimalPrecisionString);
                AskExch = tick.oe;
                ASize = tick.os.ToString();
            }
        }
    }

    public class DataGridFill
    {
        public string xTime { get; set; }
        public string Symbol { get; set; }
        public string xSide { get; set; }
        public int xSize { get; set; }
        public string xPrice { get; set; }
        public long Id { get; set; }

        public DataGridFill(Trade trade, string decimalPrecisionString)
        {
            xTime = trade.xtime.ToString();
            Symbol = trade.symbol;
            xSide = trade.side ? "BUY" : "SELL";
            xSize = trade.xsize;
            xPrice = trade.xprice.ToString(decimalPrecisionString);
            Id = trade.id;
        }
    }

    public class DataGridOrder
    {
        public int Time { get; set; }
        public string Symbol { get; set; }
        public string Side { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public long Id { get; set; }

        public DataGridOrder(Order order)
        {
            Time = order.time;
            Symbol = order.symbol;
            Side = order.side ? "BUY" : "SELL";
            Size = order.size;
            Type = order.isMarket ? "Mkt" : (order.isLimit ? "Lmt" : "Stp");
            Price = order.isStop ? order.stopp : (order.isTrail ? order.trail : order.price);
            Id = order.id;
        }
    }

    public class DataGridPosition
    {
        public string Time { get; set; }
        public string Symbol { get; set; }
        public string Side { get; set; }
        public int Size { get; set; }
        public string AvgPrice { get; set; }
        public string Profit { get; set; }
        public string Points { get; set; }

        public DataGridPosition(string nowTime, PositionImpl mypos, decimal cpl, decimal cpt, string decimalPrecisionString)
        {
            Time = nowTime;
            Symbol = mypos.Symbol;
            Side = mypos.isFlat ? "FLAT" : (mypos.isLong ? "LONG" : "SHORT");
            Size = mypos.Size;
            AvgPrice = mypos.AvgPrice.ToString(decimalPrecisionString);
            Profit = cpl.ToString("C2");
            Points = cpt.ToString(decimalPrecisionString);
        }
    }
    
    #endregion

}
