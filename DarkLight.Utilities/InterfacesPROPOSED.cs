using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using OxyPlot;
using TradeLink.API;

namespace DarkLight.Utilities
{
    public interface IHub
    {
        void Start();
        void Stop();
        void Publish(DarkLightEventArgs e);
        Dictionary<byte[], Action<DarkLightEventArgs>> EventDict { get; set; }
    }

    public interface IActor
    {
        ActorType Type { get; }

        void Start();
        void Stop();
    }

    public interface IDarkLightBroker : IActor
    {
        HistSim HistSimulator { get; set; }
    }

    public interface IDarkLightResponse : IActor
    {
        Response ResponseInstance { get; set; }
        string[] Indicators { get; }

        void Reset();
    }

    public interface IReporter : IActor
    {
        #region Control Properties

        void RegisterDispatcher(byte[] dispatchableType, Dispatcher dispatcher);
        void UpdateReportPlots();

        string ReportName { get; set; }
        ObservableCollection<KeyValuePair<string, string>> ResultsList { get; set; }
        DarkLightResults Results { get; set; }
        ObservableCollection<ObservableMessage> Messages { get; set; }
        ObservableCollection<DataGridTick> TickTable { get; set; }
        DataTable IndicatorTable { get; set; }
        ObservableCollection<DataGridPosition> PositionTable { get; set; }
        ObservableCollection<DataGridOrder> OrderTable { get; set; }
        ObservableCollection<DataGridFill> FillTable { get; set; }
        ObservableCollection<TimePlot> Plots { get; set; }
        PlotModel ReportPlots { get; set; }

        #endregion

    }


}
