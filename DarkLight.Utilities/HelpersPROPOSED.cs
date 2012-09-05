using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using DarkLight.Analytics.Models;
using TradeLink.API;

namespace DarkLight.Utilities
{

    public class HubConfigurationModel
    {
        public HubType Type;

        public HubConfigurationModel(HubType type)
        {
            Type = type;
        }
    }

    public class BrokerConfigurationModel : ActorConfigurationModel
    {
        public BrokerType Type;

        //Sim Settings
        public PlayTo PlayToValue;
        public List<string> TickFiles;
        public bool SimUseBidAskFills;
        //public AutoResetEvent ARE;
        public bool SessionRunning;

        //Live Settings
        //TODO

        public BrokerConfigurationModel(BrokerType type)
        {
            Type = type;
        }
    }

    public class ResponseConfigurationModel : ActorConfigurationModel
    {
        public List<Response> ResponseList = new List<Response>();
    }

    public class ReportConfigurationModel : ActorConfigurationModel
    {
        public ReportType Type;
        public Response ResponseInstance { get; set; }
        public ActivityModel ActivityInstance { get; set; }
        public string ReportName { get; set; }
        //public Action UpdatePlots { get; set; }

        //Sim Settings
        public List<string> TickFiles { get; set; }
        public PlayTo PlayToValue { get; set; }
    }

    public class ActorConfigurationModel
    {
        public List<byte[]> SubscriptionList;
        public bool FilterMode;
    }

    public class BrokerInfo
    {
        public string SimPrettyTickFiles;
    }

    public class ResponseInfo
    {
        public string ResponseName;
        public string[] Indicators;
    }

    public class DarkLightEventArgs
    {
        public bool Boolean;
        public Color Color;
        public decimal Decimal;
        public double Double;
        public int Integer;
        public long Long;
        public Order Order;
        public byte[] Type;
        public ServiceType SenderType;
        public TransitionType TransitionType;
        public BrokerInfo BrokerInfo;
        public ResponseInfo ResponseInfo;
        public string String;
        public Tick Tick;
        public Trade Trade;

        public DarkLightEventArgs(byte[] type)
        {
            Type = type;
        }
    }

    //serialization testing
    public class TickString
    {
        public string Tick;

        public TickString(string tick)
        {
            Tick = tick;
        }
    }
}
