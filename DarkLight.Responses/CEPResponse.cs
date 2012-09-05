using System.ComponentModel;
using TradeLink.Common;
using TradeLink.API;
using com.espertech.esper.client;

namespace DarkLight.Responses
{
    public class CEPResponse : ResponseTemplate 
    {
        // CEP engine stuff:
        private EPServiceProvider _provider;
        private EPRuntime _runtime;

        private BarListTracker _barListTracker = new BarListTracker();
        private PositionTracker _positionTracker = new PositionTracker();

        // user parameters that are displayed/fetched when ParamPrompt is called
        [Description("All responses using the same Trading Space will share a CEP engine.")]
        public string TradingSpace { get; set; }

        [Description("Length of window used for statistical calculations.")]
        public int WindowLength { get; set; }

          
        public CEPResponse()
        {
            // CEP engine configruation:
            var configuration = new Configuration();
            configuration.AddEventType("Tick", typeof(Tick).FullName);
            configuration.AddEventType("Bar", typeof(Bar).FullName);
            _provider = EPServiceProviderManager.GetProvider(TradingSpace, configuration);
            _runtime = _provider.EPRuntime;

            // For working with bar data:
            _barListTracker.GotNewBar += new SymBarIntervalDelegate(_barListTracker_GotNewBar);
        }

        public void ConfigureEngine(EPAdministrator administrator)
        {
            var averageStatement = administrator.AverageStatement(WindowLength);
            var standardDeviationStatement = administrator.StandardDeviationStatement(WindowLength);
            var normalizedPriceStatement = administrator.NormalizedPriceStatement();
            var equalWeightIndexStatement = administrator.EqualWeightIndexStatement();
            var deviationFromIndexStatement = administrator.DeviationFromIndexStatement();
            var maxDeviationFromIndexStatement = administrator.MaxDeviationFromIndexStatement();
            maxDeviationFromIndexStatement.Events += new UpdateEventHandler(maxDeviationFromIndexStatement_Events);
        }

        void _barListTracker_GotNewBar(string symbol, int interval)
        {
            var bar = _barListTracker[symbol].RecentBar;
            _runtime.SendEvent(bar);
        }
        
        void maxDeviationFromIndexStatement_Events(object sender, UpdateEventArgs e)
        {
            if (e.NewEvents == null)
            {
                return; // ignore old events for events leaving the window
            }

            var newEvent = e.NewEvents[0];
            var symbol = newEvent["symbol"];
            var deviation = newEvent["maxDeviation"];
        }

        public override void GotFill(Trade f)
        {
            _positionTracker.GotFill(f);
            base.GotFill(f);
        }

        public override void GotMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
        {
            base.GotMessage(type, source, dest, msgid, request, ref response);
        }

        public override void GotOrder(Order o)
        {
            base.GotOrder(o);
        }

        public override void GotOrderCancel(long id)
        {
            base.GotOrderCancel(id);
        }

        public override void GotPosition(Position p)
        {
            _positionTracker.Adjust(p);
            base.GotPosition(p);
        }

        public override void GotTick(Tick k)
        {
            _runtime.SendEvent(k);
            _barListTracker.GotTick(k);
            base.GotTick(k);
        }
    }
}
