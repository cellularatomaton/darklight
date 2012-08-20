using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using DarkLight.Utilities;
using TradeLink.Common;
using TradeLink.API;
using TradeLink.AppKit;

namespace DarkLight.Analytics.Models
{
    public class OptimizationModel : INotifyPropertyChanged, IDisposable
    {
        #region Private Members

        string _responseName = "";
        Response myres = null;
        PlayTo _playTo = PlayTo.Hour;
        bool _useBidAskFills = false;
        string _programName = "DarkLight.Analytics";
        bool _showticks = true;
        int _time = 0;
        int _date = 0;
        //int ctime = 0;
        string _playToString = "Play +";
        string _nowTime = "0";
        string _status = "";
        decimal _rfr = 0.00m;
        decimal _comm = .0012m;
        private int _percentComplete = 0;

        Dictionary<string, PositionImpl> _positionList = new Dictionary<string, PositionImpl>();
        List<Trade> _tradeList = new List<Trade>();
        List<string> _historicalDataFiles = new List<string>();

        BackgroundWorker _backgroundWorker;
        HistSim _historicalSimulator;
        Broker _broker;
        ResultsModel _resultsModel;
        Action<Results> _onCompleted;
        Action<string> _onStatusUpdate;
        Action<int> _onPercentComplete;

        #endregion

        #region Public Members

        public int PercentComplete
        {
            get { return _percentComplete; }
            set
            {
                if (value != _percentComplete)
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

        #endregion

        #region Constructors

        public OptimizationModel(Action<string> onStatusUpdate)
        {
            _onStatusUpdate = onStatusUpdate;
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

        public void Play(PlayTo pt, Action<Results> onCompleted, Action<int> onPercentComplete)
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
                _resultsModel.Clear();
                disposeAll();
                initializeSim();
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

            _broker.UseBidAskFills = _useBidAskFills;
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
            }

            // send to response
            if (myres != null)
                myres.GotTick(t);

            var numberTicksProcessed = Convert.ToDouble(_historicalSimulator.TicksProcessed);
            var totalNumberTicks = Convert.ToDouble(_historicalSimulator.TicksPresent);
            PercentComplete = Convert.ToInt32(Math.Round((numberTicksProcessed / totalNumberTicks) * 100));
        }

        void broker_GotFill(Trade t)
        {
            if (myres != null)
                myres.GotFill(t);
            _tradeList.Add(t);
            PositionImpl mypos = new PositionImpl(t);
            //decimal cpl = 0;
            //decimal cpt = 0;
            if (!_positionList.TryGetValue(t.symbol, out mypos))
            {
                mypos = new PositionImpl(t);
                _positionList.Add(t.symbol, mypos);
            }
            else
            {
                //cpt = Calc.ClosePT(mypos, t);
                //cpl = mypos.Adjust(t);
                _positionList[t.symbol] = mypos;
            }
        }

        void broker_GotOrder(Order o)
        {
            if (myres != null)
                myres.GotOrder(o);
        }
        
        void bindresponseevents()
        {
            myres.SendDebugEvent += myres_GotDebug;
            myres.SendCancelEvent += myres_CancelOrderSource;
            myres.SendOrderEvent += myres_SendOrder;

            // Events swallowed:
            myres.SendIndicatorsEvent += (idx, data) => { };
            myres.SendMessageEvent += (MessageTypes type, long source, long dest, long msgid, string request, ref string response) => { };
            myres.SendBasketEvent += (basket, id) => { };
            myres.SendChartLabelEvent += (price, time, label, color) => { };
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
            debug(msg);
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
            _onStatusUpdate(_status);
        }

        void debug(string msg)
        {
            //_onMessageUpdate(msg);
        }

        bool isTIK(string path) { return System.Text.RegularExpressions.Regex.IsMatch(path, TikConst.EXT, System.Text.RegularExpressions.RegexOptions.IgnoreCase); }

        void PlayComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            _resultsModel.Clear();
            var newResults = TradeResult.ResultsFromTradeList(_tradeList);
            var results = Results.FetchResults(newResults, _rfr, _comm, debug);
            _onCompleted(results);

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

        void disposeHistoricalSim()
        {
            if (_historicalSimulator != null)
            {
                _historicalSimulator.GotTick -= historicalSimulator_GotTick;
            }
        }

        void disposeBroker()
        {
            if (_broker != null)
            {
                _broker.GotFill -= broker_GotFill;
                _broker.GotOrder -= broker_GotOrder;
                _broker.GotOrderCancel -= broker_GotOrderCancel;
            }
        }

        void disposeResponse()
        {
            if (myres != null)
            {
                myres.SendDebugEvent -= myres_GotDebug;
                myres.SendCancelEvent -= myres_CancelOrderSource;
                myres.SendOrderEvent -= myres_SendOrder;
            }
        }

        void disposeBackgroundWorker()
        {
            if (_backgroundWorker != null)
            {
                _backgroundWorker.DoWork -= play;
                _backgroundWorker.RunWorkerCompleted -= PlayComplete;
                _backgroundWorker.Dispose();
            }
        }

        void disposeAll()
        {
            disposeHistoricalSim();
            disposeBroker();
            disposeBackgroundWorker();
            disposeResponse();
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
                    disposeAll();
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
        ~OptimizationModel()
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
}
