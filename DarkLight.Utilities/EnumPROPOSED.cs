using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Utilities
{
    public enum ActorType
    {
        Broker,
        Market,
        Portfolio,
        Response,
        Report,
        Risk
    }

    public enum HubType
    {
        Local,
        Zero,
        Mixed
    }

    public enum BrokerType
    {
        Sim,
        Live
    }

    public enum ReportType
    {
        Streaming,
        Batch
    }

    public enum ServiceType
    {
        Broker,
        PortfolioManager,
        Response,
        Reporter,
        RiskManager
    }

    public enum TransitionType
    {
        Begin,
        End
    }

    public class EventType
    {
        public static readonly byte[] Basket = new byte[] { 1 };
        public static readonly byte[] CancelOrder = new byte[] { 2 };
        public static readonly byte[] CancelOrderAck = new byte[] { 3 };
        public static readonly byte[] ChartLabel = new byte[] { 4 };
        public static readonly byte[] Fill = new byte[] { 5 };
        public static readonly byte[] Indicator = new byte[] { 6 };
        public static readonly byte[] Message = new byte[] { 7 };
        public static readonly byte[] Order = new byte[] { 8 };
        public static readonly byte[] OrderAck = new byte[] { 9 };
        public static readonly byte[] Plot = new byte[] { 10 };
        public static readonly byte[] Position = new byte[] { 11 };
        public static readonly byte[] ServiceTransition = new byte[] { 12 };
        public static readonly byte[] SessionEnd = new byte[] { 13 };
        public static readonly byte[] Status = new byte[] { 14 };
        public static readonly byte[] Tick = new byte[] { 15 };
    }
}
