using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;
using DarkLight.Utilities;
using OxyPlot;
using TradeLink.API;
using TradeLink.Common;

namespace DarkLight.Analytics.Models
{

    public class ReportModel2 : Actor, INotifyPropertyChanged, IDisposable, IReporter
    {
        #region Public Members

        protected Dictionary<string, TimePlot> _plotMap = new Dictionary<string, TimePlot>();


        private string _reportName;
        public string ReportName
        {
            get { return _reportName; }
            set
            {
                if (value != _reportName)
                {
                    _reportName = value;
                    NotifyPropertyChanged("ReportName");
                }
            }
        }

        ObservableCollection<KeyValuePair<string, string>> _resultsList = new ObservableCollection<KeyValuePair<string, string>>();
        public ObservableCollection<KeyValuePair<string, string>> ResultsList
        {
            get { return _resultsList; }
            set
            {
                if (value != _resultsList)
                {
                    _resultsList = value;
                    NotifyPropertyChanged("ResultsList");
                }
            }
        }

        DarkLightResults _results;
        public DarkLightResults Results
        {
            get { return _results; }
            set
            {
                if (value != _results)
                {
                    _results = value;
                    NotifyPropertyChanged("Results");
                }
            }
        }

        ObservableCollection<ObservableMessage> _messages = new ObservableCollection<ObservableMessage>();
        public ObservableCollection<ObservableMessage> Messages
        {
            get { return _messages; }
            set
            {
                if (value != _messages)
                {
                    _messages = value;
                    NotifyPropertyChanged("Messages");
                }
            }
        }

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

        private PlotModel _reportPlots;
        public PlotModel ReportPlots
        {
            get { return _reportPlots; }
            set
            {
                if (value != _reportPlots)
                {
                    _reportPlots = value;
                    NotifyPropertyChanged("ReportPlots");
                }
            }
        }

        #endregion

        #region Protected Members

        protected bool _isBatch;
        protected ActivityModel _activityModel;
        protected Dictionary<byte[], Dispatcher> _dispatchingMap = new Dictionary<byte[], Dispatcher>();
        protected Dictionary<byte[], Action<DarkLightEventArgs>> _actionMap = new Dictionary<byte[], Action<DarkLightEventArgs>>();

        #endregion

        #region Constructors

        public ReportModel2(IHub hub, ReportConfigurationModel config)
            : base(hub, config)
        {
            _activityModel = config.ActivityInstance;

            // Default each dispatchable type to the current thread's dispatcher.  Any controls which register a dispatcher will change this.
            foreach (var eventType in config.SubscriptionList)
            {
                _dispatchingMap[eventType] = Dispatcher.CurrentDispatcher;
            }
        }

        #endregion

        #region Public Methods

        // Register a dispatcher from the control thread.
        public void RegisterDispatcher(byte[] eventType, Dispatcher dispatcher)
        {
            _dispatchingMap[eventType] = dispatcher;
        }

        public static string PrettyTickDataFiles(List<string> historicalDataFiles)
        {
            string[] list = new string[historicalDataFiles.Count];
            for (int i = 0; i < historicalDataFiles.Count; i++)
                list[i] = System.IO.Path.GetFileNameWithoutExtension(historicalDataFiles[i]);
            return list.Length > 0 ? "[" + string.Join(",", list) + "]" : "[?]";
        }

        public void UpdateReportPlots()
        {
            double maxTime = double.MinValue;
            double minTime = double.MaxValue;
            double maxValue = double.MinValue;
            double minValue = double.MaxValue;

            var plotModel = new PlotModel();
            var dateTimeAxis = new TimeSpanAxis();
            dateTimeAxis.SetColors();
            plotModel.Axes.Add(dateTimeAxis);
            var linearAxis = new LinearAxis();
            linearAxis.SetColors();
            plotModel.Axes.Add(linearAxis);

            var plots = Plots.Where(p => p.Selected);
            foreach (var _timePlot in plots)
            {
                var lineSeries = new LineSeries();
                var plotColor = _timePlot.PointColor;
                lineSeries.Color = OxyColor.FromArgb(plotColor.A, plotColor.R, plotColor.G, plotColor.B);
                lineSeries.MarkerFill = OxyColor.FromArgb(plotColor.A, plotColor.R, plotColor.G, plotColor.B);
                lineSeries.MarkerType = MarkerType.None;
                lineSeries.StrokeThickness = 1;
                lineSeries.DataFieldX = "Time";
                lineSeries.DataFieldY = "Value";

                var orderedPoints = _timePlot.PlotPoints
                    .GroupBy(point => point.Time)
                    .Select(group => group.First())
                    .OrderBy(point => point.Time)
                    .ToList();
                foreach (var _point in orderedPoints)
                {
                    var time = _point.Time.TimeOfDay.TotalSeconds;
                    var value = Convert.ToDouble(_point.Value);
                    maxTime = maxTime < time ? time : maxTime;
                    minTime = time < minTime ? time : minTime;
                    maxValue = maxValue < value ? value : maxValue;
                    minValue = value < minValue ? value : minValue;

                    lineSeries.Points.Add(new DataPoint { X = time, Y = value });
                }
                plotModel.Series.Add(lineSeries);
            }

            dateTimeAxis.AbsoluteMaximum = maxTime;
            dateTimeAxis.AbsoluteMinimum = minTime;
            linearAxis.AbsoluteMaximum = maxValue;
            linearAxis.AbsoluteMinimum = minValue;

            plotModel.SetColors();
            ReportPlots = plotModel;
        }

        #endregion

        #region Protected Subscription Methods

        protected override void EventFilter(DarkLightEventArgs de)
        {
            if (_dispatchingMap[de.Type].CheckAccess() || _isBatch)
            {
                _actionDict[de.Type](de);
            }
            else
            {
                _dispatchingMap[de.Type].Invoke(_actionDict[de.Type], DispatcherPriority.Normal, de);
            }
        }

        #endregion

        #region Protected Dispatch Methods

        protected virtual void ProcessIndicators(DarkLightEventArgs de)
        {
            // Do you thang...
        }

        protected virtual void ProcessChartLabel(DarkLightEventArgs de)
        {
            // Do you thang...
        }

        protected virtual void ProcessPosition(DarkLightEventArgs de)
        {
            // Do you thang...
        }

        protected virtual void ProcessOrder(DarkLightEventArgs de)
        {
            // Do you thang...
        }

        protected virtual void ProcessFill(DarkLightEventArgs de)
        {
            // Do you thang...
        }

        protected virtual void ProcessTick(DarkLightEventArgs de)
        {
            // Do you thang...
        }

        protected virtual void ProcessStatusUpdate(DarkLightEventArgs de)
        {
            // Do you thang...
        }

        protected virtual void ProcessMessage(DarkLightEventArgs de)
        {
            // Do you thang...
        }

        #endregion

        #region Implementation of IDisposable

        void DisposeAll()
        {

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
        ~ReportModel2()
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

    public class BatchReportModel2 : ReportModel2
    {
        #region Private Members

        Dictionary<string, PositionImpl> _positionList = new Dictionary<string, PositionImpl>();
        List<Trade> _tradeList = new List<Trade>();
        //ResultsModel _resultsModel = new ResultsModel();
        StringBuilder _messageBuilder = new StringBuilder();
        //Action _updatePlots;

        DataTable _indicatorTable = new DataTable();// = new DataTable("indicatorTable");
        ObservableCollection<DataGridTick> _tickCollection = new ObservableCollection<DataGridTick>();
        ObservableCollection<DataGridPosition> _positionCollection = new ObservableCollection<DataGridPosition>();
        ObservableCollection<DataGridOrder> _orderCollection = new ObservableCollection<DataGridOrder>();
        ObservableCollection<DataGridFill> _fillCollection = new ObservableCollection<DataGridFill>();

        bool _missingIndicatorWarn = true;
        double _ticksPresent;
        string _responseName = "";
        string _prettyTickDataFiles = "";
        string _nowTime = "0";
        string _programName = "Incepto.Analytics";
        const string _decimalPrecisionString = "N2";
        //HistSim _histSim;

        #endregion

        #region Constructors

        public BatchReportModel2(IHub hub, ReportConfigurationModel config)
            : base(hub, config)
        {
            _isBatch = true;
            ReportName = config.ReportName;
        }
        #endregion

        #region Protected Subscription Methods

        protected override void OnChartLabel(DarkLightEventArgs de)
        {
            var col = de.Color;
            var price = de.Decimal;
            var bar = de.Integer;
            var label = de.String;

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

            if (!_plotMap.ContainsKey(label))
            {
                _plotMap.Add(label, plot);
            }
            else
            {
                point = plot.PlotPoints[0];
                _plotMap[label].PlotPoints.Add(point);
            }
        }

        protected override void OnIndicator(DarkLightEventArgs de)
        {
            var index = de.Integer;
            var indicators = de.String;

            string[] values = indicators.Split(',');

            try
            {
                _indicatorTable.Rows.Add(values);
            }
            catch (ArgumentException ex)
            {
                if (_missingIndicatorWarn && ex.Message.Contains("array is longer than the number of columns"))
                {
                    _missingIndicatorWarn = false;
                    PublishMessage("Your indicator names do not match the number of indicators you sent with sendindicators.");
                    PublishMessage("Check to make sure you do not have commas in your sendindicator values.");
                    PublishStatus("User error in specifying indicators.");
                }
            }
        }

        protected override void OnFill(DarkLightEventArgs de)
        {
            var trade = de.Trade;

            _tradeList.Add(trade);
            PositionImpl mypos = new PositionImpl(trade);
            decimal cpl = 0;
            decimal cpt = 0;
            if (!_positionList.TryGetValue(trade.symbol, out mypos))
            {
                mypos = new PositionImpl(trade);
                _positionList.Add(trade.symbol, mypos);
            }
            else
            {
                cpt = Calc.ClosePT(mypos, trade);
                cpl = mypos.Adjust(trade);
                _positionList[trade.symbol] = mypos;
            }

            _positionCollection.Add(new DataGridPosition(_nowTime, mypos, cpl, cpt, _decimalPrecisionString));
            _fillCollection.Add(new DataGridFill(trade, _decimalPrecisionString));
        }

        protected override void OnMessage(DarkLightEventArgs de)
        {
            var message = de.String;
            _messageBuilder.AppendFormat("{0}: {1}{2}", _nowTime, message, Environment.NewLine);
        }

        protected override void OnOrder(DarkLightEventArgs de)
        {
            _orderCollection.Add(new DataGridOrder(de.Order));
        }

        protected override void OnPosition(DarkLightEventArgs de)
        {

        }

        protected override void OnServiceTransition(DarkLightEventArgs de)
        {
            switch (de.SenderType)
            {
                case  ServiceType.Broker:
                    if (de.TransitionType == TransitionType.Begin)
                        _prettyTickDataFiles = de.BrokerInfo.SimPrettyTickFiles;
                    else if (de.TransitionType == TransitionType.End)
                        backtestComplete();
                    break;
                case ServiceType.Response:
                    if (de.TransitionType == TransitionType.Begin)
                    {
                        _responseName = de.ResponseInfo.ResponseName;
                        initializeIndicators(de.ResponseInfo.Indicators);
                    }
                    break;
            }            
        }

        protected override void OnStatus(DarkLightEventArgs de)
        {
            _activityModel.Status = de.String;
        }

        protected override void OnTick(DarkLightEventArgs de)
        {
            var tick = de.Tick;

            _nowTime = tick.time.ToString();

            _tickCollection.Add(new DataGridTick(tick, _decimalPrecisionString));

            int percentComplete = de.Integer; //hack
            _activityModel.PercentComplete = percentComplete;
        }

        #endregion

        #region Private Methods

        void backtestComplete()
        {
            string[] r = _messageBuilder.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string l in r)
            {
                //_activityModel.Messages.Add(new ObservableMessage {Message = l});
                Messages.Add(new ObservableMessage { Message = l });
            }

            IndicatorTable = _indicatorTable;
            TickTable = _tickCollection;
            OrderTable = _orderCollection;
            FillTable = _fillCollection;
            PositionTable = _positionCollection;

            Plots = new ObservableCollection<TimePlot>(_plotMap.Values);
            //_updatePlots();

            //ResultsTable = _resultsModel.RunResults(_responseName + "." + _prettyTickDataFiles, _tradeList);

            var name = _responseName + "." + _prettyTickDataFiles;
            var resultInstance = DarkLightResults.GetDarkLightResults(name, _tradeList, PublishMessage, 0.0001m, 0.0012m);
            ResultsList = new ObservableCollection<KeyValuePair<string, string>>(PlottingUtilities.GetFieldAndPropertyValueList(resultInstance));
            Results = resultInstance;

            var de = new DarkLightEventArgs(EventType.SessionEnd);
            _publish(de);

            _activityModel.NumberTestsCompleted = _activityModel.NumberTestsCompleted + 1;

        }

        void initializeTables()
        {
            _positionCollection.Clear();
            _orderCollection.Clear();
            _fillCollection.Clear();
            _tickCollection.Clear();

            //_resultsModel.Clear();

        }

        void initializeIndicators(string[] indicators)
        {
            // clear existing indicators
            _indicatorTable.Clear();
            _indicatorTable.Columns.Clear();

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
                        PublishMessage("You have duplicate column name: " + indicators[i] + " defined in your response.  Please remove this and try again.");
                    }
                }
            }
        }

        #endregion

    }


}
