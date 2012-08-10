using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.IO;
using DarkLight.Utilities;
using TradeLink.Common;
using TradeLink.API;
using TradeLink.AppKit;

namespace DarkLight.Analytics.Models
{
    public class BacktestingModel : INotifyPropertyChanged, IDisposable
    {
        #region Private Members

        string _responseName = "";
        Response myres = null;
        PlayTo _playTo = PlayTo.Hour;
        int _decimalPrecision = 2;
        string _decimalPrecisionString = "N2";
        bool _useBidAskFills = false;
        string _programName = "Incepto.Analytics";
        bool _showticks = true;
        int _time = 0;
        int _date = 0;
        //int ctime = 0;
        string _playToString = "Play +";
        string _nowTime = "0";
        string _status = "";
        private int _percentComplete = 0;

        StringBuilder _messageBuilder = new StringBuilder();
        DataTable _tickTable = new DataTable("tickTable");
        DataTable _indicatorTable = new DataTable("indicatorTable");
        DataTable _positionTable = new DataTable("positionTable");
        DataTable _orderTable = new DataTable("orderTable");
        DataTable _fillTable = new DataTable("fillTable");
        
        Dictionary<string, PositionImpl> _positionList = new Dictionary<string, PositionImpl>();
        List<Trade> _tradeList = new List<Trade>();
        List<string> _historicalDataFiles = new List<string>();

        BackgroundWorker _backgroundWorker;
        HistSim _historicalSimulator;
        Broker _broker;
        ResultsModel _resultsModel;
        Action _onCompleted;
        Action<int> _onPercentComplete;
        Action<string> _onStatusUpdate;
        Action<string> _onMessageUpdate;

        #endregion

        #region Public Members

        public string Status
        {
            get { return _status; }
            set
            {
                if(value != _status)
                {
                    _status = value;
                    if (_onStatusUpdate != null)
                    {
                        _onStatusUpdate(_status);
                    }
                    NotifyPropertyChanged("Status");
                }
            }
        }

        public int PercentComplete 
        {
            get { return _percentComplete; }
            set
            {
                if(value != _percentComplete)
                {
                    _percentComplete = value;
                    if (_onPercentComplete != null)
                    {
                        _onPercentComplete(_percentComplete);
                    }
                    NotifyPropertyChanged("PercentComplete");
                }
            }
        }

        public Response ResponseInstance
        {
            get { return myres; }
            set
            {
                if (value != myres)
                {
                    myres = value;
                    NotifyPropertyChanged("ResponseInstance");
                }
            }
        }

        private DataTable _completedTickTable;
        public DataTable TickTable
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

        private DataTable _completedPositionTable;
        public DataTable PositionTable
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

        private DataTable _completedOrderTable;
        public DataTable OrderTable
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

        private DataTable _completedFillTable;
        public DataTable FillTable
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
                if(value != _completedPlots)
                {
                    _completedPlots = value;
                    NotifyPropertyChanged("Plots");
                }
            }
        }

        #endregion

        #region Constructors

        public BacktestingModel(Action<string> onStatusUpdate, Action<string> onMessageUpdate)
        {
            _onStatusUpdate = onStatusUpdate;
            _onMessageUpdate = onMessageUpdate;
            initializeSim();
        }

        #endregion

        #region Public Methods

        public void LoadResponse(Response response)
        {
            ResponseInstance = response;
            _responseName = response.FullName;
            status(_responseName + " is current response.");
            bindresponseevents();
            myres.ID = 0;
            try
            {
                myres.Reset();
                initializeIndicators();
            }
            catch (Exception ex)
            {
                debug("An error occured inside your response Reset method: ");
                debug(ex.Message + ex.StackTrace);
            }
            hasprereq();
        }

        public bool LoadTickData(List<string> paths)
        {
            bool success = false;
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
            success = loadsim();
            success = success && hasprereq();
            return success;
        }

        public void Play(PlayTo pt, Action onCompleted, Action<int> onPercentComplete)
        {
            _onCompleted = onCompleted;
            _onPercentComplete = onPercentComplete;
            if (!hasprereq(true))
                return;
            _backgroundWorker.RunWorkerAsync(pt);
            status("Playing next " + pt.ToString().Replace(_playToString, string.Empty) + "...");
        }

        public void Reset()
        {
            try
            {
                _positionList.Clear();
                _tradeList.Clear();

                _messageBuilder = new StringBuilder();

                TickTable.Clear();
                TickTable.Columns.Clear();
                _tickTable.Clear();
                _tickTable.Columns.Clear();

                PositionTable.Clear();
                PositionTable.Columns.Clear();
                _positionTable.Clear();
                _positionTable.Columns.Clear();

                OrderTable.Clear();
                OrderTable.Columns.Clear();
                _orderTable.Clear();
                _orderTable.Columns.Clear();

                FillTable.Clear();
                FillTable.Columns.Clear();
                _fillTable.Clear();
                _fillTable.Columns.Clear();

                ResultsTable.Clear();
                ResultsTable.Columns.Clear();
                _resultsModel.Clear();

                Plots.Clear();
                _plotMap.Clear();

                initializeSim();
                myres.Reset();
                initializeIndicators();
                _nowTime = "0";
            }
            catch (Exception ex)
            {
                status("An error occured, try again.");
                debug("Reset error: " + ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region Private Methods

        void initializeSim()
        {
            
            _backgroundWorker = new BackgroundWorker();
            _historicalSimulator = new MultiSimImpl();
            _broker = new Broker();
            _resultsModel = new ResultsModel();
            
            _decimalPrecisionString = "N" + _decimalPrecision;
            _broker.UseBidAskFills = _useBidAskFills;
            initializeTables();
            _backgroundWorker.DoWork += play;
            _backgroundWorker.WorkerReportsProgress = false;
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.RunWorkerCompleted += PlayComplete;

            debug(Util.TLSIdentity());
            debug(RunTracker.CountNewGetPrettyRuns(_programName, Util.PROGRAM));
        }

        bool loadsim()
        {
            
            _historicalSimulator = new MultiSimImpl(_historicalDataFiles.ToArray());
            _historicalSimulator.GotTick += historicalSimulator_GotTick;

            _broker = new Broker();
            _broker.GotOrder += broker_GotOrder;
            _broker.GotFill += broker_GotFill;
            _broker.UseBidAskFills = _useBidAskFills;
            _broker.GotOrderCancel += broker_GotOrderCancel;
            
            try
            {
                status("Loaded tickdata: " + prettyTickDataFiles());
                return true;
            }
            catch (IOException ex)
            {
                if (ex.Message.Contains("used by another process"))
                {
                    status("Simulation file still in use.");
                    debug("Try again, one of following in use: " + string.Join(",", _historicalDataFiles.ToArray()));
                }
                return false;
            }
        }

        void play(object sender, DoWorkEventArgs e)
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
                //case PlayTo.Custom:
                //    ctime = getcusttime();
                //    if (ctime == 0)
                //    {
                //        _playTo = PlayTo.OneSec;
                //        val = 0;
                //        status("Invalid custom time, playing to next second.");
                //    }
                //    else
                //        val = date + ctime;
                //    break;
            }
            _historicalSimulator.PlayTo(val);
        }

        //int getcusttime()
        //{
        //    if (InvokeRequired)
        //        return (int)Invoke(new intvoiddel(getcusttime));
        //    else
        //    {
        //        string cts = Microsoft.VisualBasic.Interaction.InputBox("Enter PlayTo Time: (eg 4:15:01pm = 161501)", "Custom Play Time", ctime.ToString(), 0, 0);
        //        int ct = 0;
        //        if (int.TryParse(cts, out ct))
        //            return ct;
        //        return 0;
        //    }
        //}

        bool hasprereq() 
        { 
            return hasprereq(true);
        }

        bool hasprereq(bool stat)
        {
            if (_backgroundWorker.IsBusy) { if (stat) status("Still playing, please wait..."); return false; }
            if (myres == null) { if (stat) status("Add response."); return false; }
            if (_historicalDataFiles.Count == 0) { if (stat) status("Add study data."); return false; }
            if (stat)
                status("Click on desired play duration to begin.");
            return true;
        }

        bool _missingIndicatorWarn = true;
        void newIndicatorRow(object[] values)
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
                    debug("Your indicator names do not match the number of indicators you sent with sendindicators.");
                    debug("Check to make sure you do not have commas in your sendindicator values.");
                    status("User error in specifying indicators.");
                }
            }
        }

        void initializeTables()
        {
            // position
            _positionTable.Columns.Add("Time");
            _positionTable.Columns.Add("Symbol");
            _positionTable.Columns.Add("Side");
            _positionTable.Columns.Add("Size");
            _positionTable.Columns.Add("AvgPrice");
            _positionTable.Columns.Add("Profit");
            _positionTable.Columns.Add("Points");

            // order
            _orderTable.Columns.Add("Time");
            _orderTable.Columns.Add("Symbol");
            _orderTable.Columns.Add("Side");
            _orderTable.Columns.Add("Size");
            _orderTable.Columns.Add("Type");
            _orderTable.Columns.Add("Price");
            _orderTable.Columns.Add("Id");

            // fill
            _fillTable.Columns.Add("xTime");
            _fillTable.Columns.Add("Symbol");
            _fillTable.Columns.Add("xSide");
            _fillTable.Columns.Add("xSize");
            _fillTable.Columns.Add("xPrice");
            _fillTable.Columns.Add("Id");

            // tick
            _tickTable.Columns.Add("Time", "".GetType());
            _tickTable.Columns.Add("Sym");
            _tickTable.Columns.Add("Trade");
            _tickTable.Columns.Add("TSize");
            _tickTable.Columns.Add("Bid");
            _tickTable.Columns.Add("Ask");
            _tickTable.Columns.Add("BSize");
            _tickTable.Columns.Add("ASize");
            _tickTable.Columns.Add("TExch");
            _tickTable.Columns.Add("BidExch");
            _tickTable.Columns.Add("AskExch");
        }

        void initializeIndicators()
        {
            // don't process invalid responses
            if ((myres == null) || (myres.Indicators == null) || (myres.Indicators.Length == 0))
                return;
            // clear existing indicators
            _indicatorTable.Clear();
            _indicatorTable.Columns.Clear();
            // load new ones
            for (int i = 0; i < myres.Indicators.Length; i++)
            {
                try
                {
                    _indicatorTable.Columns.Add(myres.Indicators[i]);
                }
                catch (DuplicateNameException)
                {
                    debug("You have duplicate column name: " + myres.Indicators[i] + " defined in your response.  Please remove this and try again.");
                }
            }
        }

        void historicalSimulator_GotTick(Tick t)
        {
            // execute pending orders
            _broker.Execute(t);
            if (_showticks)
            {
                _date = t.date;
                _time = t.time;
                // get time for display
                _nowTime = t.time.ToString();

                // don't display ticks for unmatched exchanges
                string time = _nowTime;
                string trade = "";
                string bid = "";
                string ask = "";
                string ts = "";
                string bs = "";
                string os = "";
                string be = "";
                string oe = "";
                string ex = "";
                if (t.isIndex)
                {
                    trade = t.trade.ToString(_decimalPrecisionString);
                }
                else if (t.isTrade)
                {
                    trade = t.trade.ToString(_decimalPrecisionString);
                    ts = t.size.ToString();
                    ex = t.ex;
                }
                if (t.hasBid)
                {
                    bs = t.bs.ToString();
                    be = t.be;
                    bid = t.bid.ToString(_decimalPrecisionString);
                }
                if (t.hasAsk)
                {
                    ask = t.ask.ToString(_decimalPrecisionString);
                    oe = t.oe;
                    os = t.os.ToString();
                }

                // add tick to grid
                newTickRow(new string[] { _nowTime, t.symbol, trade, ts, bid, ask, bs, os, ex, be, oe });
            }

            // send to response
            if (myres != null)
                myres.GotTick(t);

            var numberTicksProcessed = Convert.ToDouble(_historicalSimulator.TicksProcessed);
            var totalNumberTicks = Convert.ToDouble(_historicalSimulator.TicksPresent);
            PercentComplete = Convert.ToInt32(Math.Round((numberTicksProcessed/totalNumberTicks)*100));
        }

        void broker_GotFill(Trade t)
        {
            if (myres != null)
                myres.GotFill(t);
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

            _positionTable.Rows.Add(_nowTime, mypos.Symbol, (mypos.isFlat ? "FLAT" : (mypos.isLong ? "LONG" : "SHORT")), mypos.Size, mypos.AvgPrice.ToString(_decimalPrecisionString), cpl.ToString("C2"), cpt.ToString(_decimalPrecisionString));
            _fillTable.Rows.Add(t.xtime.ToString(), t.symbol, (t.side ? "BUY" : "SELL"), t.xsize, t.xprice.ToString(_decimalPrecisionString), t.id);
        }

        void broker_GotOrder(Order o)
        {
            if (myres != null)
                myres.GotOrder(o);
            _orderTable.Rows.Add(o.time, o.symbol, (o.side ? "BUY" : "SELL"), o.size, (o.isMarket ? "Mkt" : (o.isLimit ? "Lmt" : "Stp")), o.isStop ? o.stopp : (o.isTrail ? o.trail : o.price), o.id);
        }
        
        void newTickRow(string[] values)
        {
            _tickTable.Rows.Add(values);
        }

        void bindresponseevents()
        {
            myres.SendDebugEvent += new DebugDelegate(myres_GotDebug);
            myres.SendCancelEvent += new LongSourceDelegate(myres_CancelOrderSource);
            myres.SendOrderEvent += new OrderSourceDelegate(myres_SendOrder);
            myres.SendIndicatorsEvent += new ResponseStringDel(myres_SendIndicators);
            myres.SendMessageEvent += new MessageDelegate(myres_SendMessage);
            myres.SendBasketEvent += new BasketDelegate(myres_SendBasket);
            myres.SendChartLabelEvent += new ChartLabelDelegate(myres_SendChartLabel);
        }

        bool _sendbaskwarn = false;
        void myres_SendBasket(Basket b, int id)
        {
            if (_sendbaskwarn) return;
            debug("Sendbasket not supported in kadina.");
            debug("To specify trading symbols, add data to study.");
            _sendbaskwarn = true;
        }

        void myres_SendChartLabel(decimal price, int bar, string label, System.Drawing.Color col)
        {
            Color wpfColor = Color.FromArgb(col.A, col.R, col.G, col.B);
            
            // Draw line plot:
            if (!_plotMap.ContainsKey(label))
            {
                var plot = new TimePlot
                {
                    Label = label,
                    PointColor = wpfColor,
                    PlotPoints = new List<TimePlotPoint>(),
                    Selected = true,
                };
                _plotMap.Add(label, plot);
            }
            var point = new TimePlotPoint
            {
                Time = Util.FT2DT(bar),
                Value = price,
            };
            _plotMap[label].PlotPoints.Add(point);
        }

        bool _sendmesswarn = false;
        void myres_SendMessage(MessageTypes type, long source, long dest, long id, string data, ref string response)
        {
            if (_sendmesswarn) return;
            _sendmesswarn = true;
            debug("SendMessage and custom messages not supported in kadina.");
        }
        
        void myres_SendIndicators(int idx, string param)
        {
            if (myres == null) return;
            if (myres.Indicators.Length == 0)
                debug("No indicators defined on response: " + myres.Name);
            else
            {
                string[] parameters = param.Split(',');
                newIndicatorRow(parameters);
            }
        }

        void myres_SendOrder(Order o, int id)
        {
            if (o.time == 0)
            {
                o.date = _date;
                o.time = _time;
            }
            _broker.SendOrderStatus(o);
        }

        void broker_GotOrderCancel(string sym, bool side, long id)
        {
            if (myres != null)
                myres.GotOrderCancel(id);
        }

        void myres_CancelOrderSource(long number, int id)
        {
            _broker.CancelOrder(number);
        }

        void myres_GotDebug(string msg)
        {
            _messageBuilder.AppendFormat("{0}: {1}{2}", _nowTime, msg, Environment.NewLine);
        }

        string prettyTickDataFiles()
        {
            string[] list = new string[_historicalDataFiles.Count];
            for (int i = 0; i < _historicalDataFiles.Count; i++)
                list[i] = System.IO.Path.GetFileNameWithoutExtension(_historicalDataFiles[i]);
            return list.Length > 0 ? "[" + string.Join(",", list) + "]" : "[?]";
        }

        void status(string msg)
        {
            Status = msg;
        }

        void debug(string msg)
        {
            _onMessageUpdate(msg);
        }

        bool isTIK(string path) 
        { 
            return System.Text.RegularExpressions.Regex.IsMatch(path, TikConst.EXT, System.Text.RegularExpressions.RegexOptions.IgnoreCase); 
        }

        void PlayComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            string[] r = _messageBuilder.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string l in r)
                debug(l);

            TickTable = _tickTable;
            IndicatorTable = _indicatorTable;
            OrderTable = _orderTable;
            FillTable = _fillTable;
            PositionTable = _positionTable;
            Plots = new ObservableCollection<TimePlot>(_plotMap.Values);
            _resultsModel.Clear();
            ResultsTable = _resultsModel.RunResults(_responseName + "." + prettyTickDataFiles(), _tradeList);

            if(_onCompleted != null)
            {
                _onCompleted();
            }
            
            if (e.Error != null)
            {
                debug(e.Error.Message + e.Error.StackTrace);
                status("Terminated because of an Exception.  See messages.");
            }
            else if (e.Cancelled) status("Canceled play.");
            else status("Reached next " + _playTo.ToString() + " at time " + KadTime);
        }

        string KadTime
        {
            get
            {
                return _nowTime != "" ? _nowTime : "(none)";
            }
        }

        #endregion

        #region Implementation of IDisposable

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
                    if (_historicalSimulator != null)
                    {
                        _historicalSimulator.GotTick -= historicalSimulator_GotTick;
                    }
                    if (_broker != null)
                    {
                        _broker.GotFill -= broker_GotFill;
                        _broker.GotOrder -= broker_GotOrder;
                        _broker.GotOrderCancel -= broker_GotOrderCancel;
                    }
                    if (_backgroundWorker != null)
                    {
                        _backgroundWorker.DoWork -= play;
                        _backgroundWorker.RunWorkerCompleted -= PlayComplete;
                        _backgroundWorker.Dispose();
                    }
                    if(myres != null)
                    {
                        myres.SendDebugEvent -= myres_GotDebug;
                        myres.SendCancelEvent -= myres_CancelOrderSource;
                        myres.SendOrderEvent -= myres_SendOrder;
                        myres.SendIndicatorsEvent -= myres_SendIndicators;
                        myres.SendMessageEvent -= myres_SendMessage;
                        myres.SendBasketEvent -= myres_SendBasket;
                        myres.SendChartLabelEvent -= myres_SendChartLabel;
                    }
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
        ~BacktestingModel()
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
    
    public enum PlayTo
    {
        LastPlayTo,
        OneSec = 1,
        ThirtySec = 30,
        OneMin = 10,
        FiveMin = 50,
        TenMin = 100,
        HalfHour = 300,
        Hour = 1000,
        TwoHour,
        FourHour,
        Custom,
        End,
    }

    


}