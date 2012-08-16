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
    public class BacktestingModel : IReportable, IDisposable
    {

        #region Public Members

        public event Action<string> StatusUpdate;
        public event Action<string> MessageUpdate;
        public event Action<Tick> GotTick;
        public event Action<Trade> GotFill;
        public event Action<Order> GotOrder;
        public event Action<long> GotOrderCancel;
        public event Action<Position> GotPosition;
        public event Action<TimePlot> GotPlot;
        public event Action<string[]> GotIndicators;
        public event Action<EngineInfo> EngineReset;
        public event Action<RunWorkerCompletedEventArgs> EngineComplete;

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

        #region Private Members

        BackgroundWorker _backgroundWorker;
        Response myres = null;
        HistSim _historicalSimulator;
        Broker _broker;

        bool _useBidAskFills = false;
        bool _sendmesswarn = false;
        bool _sendbaskwarn = false;
        int _time = 0;
        int _date = 0;
        string _responseName = "";
        string _playToString = "Play +";

        PlayTo _playTo = PlayTo.Hour;        
        List<string> _historicalDataFiles = new List<string>();

        #endregion

        #region Constructors

        public BacktestingModel()
        {
            initializeSim();
        }

        #endregion

        #region Public Methods

        public void LoadResponse(Response response)
        {
            ResponseInstance = response;
            _responseName = response.FullName;
            StatusUpdate(_responseName + " is current response.");
            bindresponseevents();
            myres.ID = 0;
            try
            {
                myres.Reset();
            }
            catch (Exception ex)
            {
                MessageUpdate("An error occured inside your response Reset method: ");
                MessageUpdate(ex.Message + ex.StackTrace);
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

        public void Play(PlayTo pt)
        {
            if (!hasprereq(true))
                return;

            var engineInfo = getEngineInfo();
            EngineReset(engineInfo);

            _backgroundWorker.RunWorkerAsync(pt);
            StatusUpdate("Playing next " + pt.ToString().Replace(_playToString, string.Empty) + "...");
        }

        public void Reset()
        {
            try
            {
                initializeSim();
                myres.Reset();
            }
            catch (Exception ex)
            {
                StatusUpdate("An error occured, try again.");
                MessageUpdate("Reset error: " + ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region Private Methods

        void initializeSim()
        {
            _backgroundWorker = new BackgroundWorker();
            _historicalSimulator = new MultiSimImpl();
            _broker = new Broker();

            _broker.UseBidAskFills = _useBidAskFills;
            _backgroundWorker.DoWork += play;
            _backgroundWorker.WorkerReportsProgress = false;
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.RunWorkerCompleted += PlayComplete;
        }

        bool loadsim()
        {
            _historicalSimulator = new MultiSimImpl(_historicalDataFiles.ToArray());
            _historicalSimulator.GotTick += historicalSimulator_GotTick;

            _broker = new Broker();
            _broker.UseBidAskFills = _useBidAskFills;
            _broker.GotOrder += broker_GotOrder;
            _broker.GotFill += broker_GotFill;
            _broker.GotOrderCancel += broker_GotOrderCancel;

            try
            {
                StatusUpdate("Loaded tickdata: " + prettyTickDataFiles());
                return true;
            }
            catch (IOException ex)
            {
                if (ex.Message.Contains("used by another process"))
                {
                    StatusUpdate("Simulation file still in use.");
                    MessageUpdate("Try again, one of following in use: " + string.Join(",", _historicalDataFiles.ToArray()));
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

        void bindresponseevents()
        {
            myres.SendDebugEvent -= myres_GotDebug;
            myres.SendCancelEvent -= myres_CancelOrderSource;
            myres.SendOrderEvent -= myres_SendOrder;
            myres.SendIndicatorsEvent -= myres_SendIndicators;
            myres.SendMessageEvent -= myres_SendMessage;
            myres.SendBasketEvent -= myres_SendBasket;
            myres.SendChartLabelEvent -= myres_SendChartLabel;

            myres.SendOrderEvent += new OrderSourceDelegate(myres_SendOrder);
            myres.SendDebugEvent += new DebugDelegate(myres_GotDebug);
            myres.SendCancelEvent += new LongSourceDelegate(myres_CancelOrderSource);
            myres.SendIndicatorsEvent += new ResponseStringDel(myres_SendIndicators);
            myres.SendMessageEvent += new MessageDelegate(myres_SendMessage);
            myres.SendBasketEvent += new BasketDelegate(myres_SendBasket);
            myres.SendChartLabelEvent += new ChartLabelDelegate(myres_SendChartLabel);

            GotTick -= myres.GotTick;
            GotOrder -= myres.GotOrder;
            GotOrderCancel -= myres.GotOrderCancel;
            GotFill -= myres.GotFill;

            GotTick += myres.GotTick;
            GotOrder += myres.GotOrder;
            GotOrderCancel += myres.GotOrderCancel;
            GotFill += myres.GotFill;
        }

        bool isTIK(string path)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(path, TikConst.EXT, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        bool hasprereq()
        {
            return hasprereq(true);
        }

        bool hasprereq(bool stat)
        {
            if (_backgroundWorker.IsBusy)
            {
                if (stat)
                    StatusUpdate("Still playing, please wait...");
                return false;
            }
            if (myres == null)
            {
                if (stat)
                    StatusUpdate("Add response.");
                return false;
            }
            if (_historicalDataFiles.Count == 0)
            {
                if (stat)
                    StatusUpdate("Add study data.");
                return false;
            }
            if (stat)
                StatusUpdate("Click on desired play duration to begin.");

            return true;
        }

        string prettyTickDataFiles()
        {
            string[] list = new string[_historicalDataFiles.Count];
            for (int i = 0; i < _historicalDataFiles.Count; i++)
                list[i] = System.IO.Path.GetFileNameWithoutExtension(_historicalDataFiles[i]);
            return list.Length > 0 ? "[" + string.Join(",", list) + "]" : "[?]";
        }

        string[] getIndicators()
        {
            string[] indicators = null;

            // don't process invalid responses
            if (!((myres == null) || (myres.Indicators == null) || (myres.Indicators.Length == 0)))
                indicators = myres.Indicators;

            return indicators;
        }

        EngineInfo getEngineInfo()
        {
            var engineInfo = new EngineInfo();

            engineInfo.Indicators = getIndicators();
            engineInfo.PlayToString = _playTo.ToString();
            engineInfo.PrettyTickFiles = prettyTickDataFiles();
            engineInfo.ResponseName = _responseName;
            //engineInfo.TotalNumberTicks = Convert.ToDouble(_historicalSimulator.TicksPresent);
            engineInfo.HistoricalSimulator = _historicalSimulator;

            return engineInfo;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////       
        ///  ENGINE

        void PlayComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            EngineComplete(e);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////       
        ///  RESPONSE

        void myres_SendOrder(Order o, int id)
        {
            if (o.time == 0)
            {
                o.date = _date;
                o.time = _time;
            }
            _broker.SendOrderStatus(o);
        }

        void myres_GotDebug(string msg)
        {
            MessageUpdate(msg);
        }

        void myres_CancelOrderSource(long number, int id)
        {
            _broker.CancelOrder(number);
        }

        void myres_SendIndicators(int idx, string param)
        {
            if (myres == null) return;
            if (myres.Indicators.Length == 0)
                MessageUpdate("No indicators defined on response: " + myres.Name);
            else
            {
                string[] parameters = param.Split(',');
                GotIndicators(parameters);
            }
        }

        void myres_SendMessage(MessageTypes type, long source, long dest, long id, string data, ref string response)
        {
            if (_sendmesswarn) return;
            _sendmesswarn = true;
            MessageUpdate("SendMessage and custom messages not supported in darklight.");
        }

        void myres_SendBasket(Basket b, int id)
        {
            if (_sendbaskwarn) return;
            MessageUpdate("Sendbasket not supported in kadina.");
            MessageUpdate("To specify trading symbols, add data to study.");
            _sendbaskwarn = true;
        }

        void myres_SendChartLabel(decimal price, int bar, string label, System.Drawing.Color col)
        {

            Color wpfColor = Color.FromArgb(col.A, col.R, col.G, col.B);

            var plot = new TimePlot
            {
                Label = label,
                PointColor = wpfColor,
                PlotPoints = new List<TimePlotPoint>(),
                Selected = true,
            };

            var point = new TimePlotPoint
            {
                Time = Util.FT2DT(bar),
                Value = price,
            };
            plot.PlotPoints.Add(point);
            GotPlot(plot);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        ///  SIMULATOR

        void historicalSimulator_GotTick(Tick t)
        {
            _date = t.date;
            _time = t.time;

            _broker.Execute(t);
            
            //fire event consumed by response and report
            GotTick(t);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        ///  BROKER

        void broker_GotFill(Trade t)
        {
            //fire event consumed by response and report
            GotFill(t);
        }

        void broker_GotOrder(Order o)
        {
            //fire event consumed by response and report
            GotOrder(o);
        }

        void broker_GotOrderCancel(string sym, bool side, long id)
        {
            //fire event consumed by response and report
            GotOrderCancel(id);
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
                    if (myres != null)
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
}
