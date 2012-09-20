using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkLight.Utilities;
using TradeLink.Common;

namespace DarkLight.Analytics.Models
{

    public class Hub : IHub
    {
        #region Private Members

        public Dictionary<byte[], Action<DarkLightEventArgs>> EventDict { get; set; }

        #endregion

        public DarkLight.Utilities.EventType EventType
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        #region Constructor

        public Hub()
        {
            EventDict = new Dictionary<byte[], Action<DarkLightEventArgs>>();
            EventDict.Add(EventType.Basket, null);
            EventDict.Add(EventType.CancelOrder, null);
            EventDict.Add(EventType.CancelOrderAck, null);
            EventDict.Add(EventType.ChartLabel, null);
            EventDict.Add(EventType.Fill, null);
            EventDict.Add(EventType.Indicator, null);
            EventDict.Add(EventType.Message, null);
            EventDict.Add(EventType.Order, null);
            EventDict.Add(EventType.OrderAck, null);
            EventDict.Add(EventType.Plot, null);
            EventDict.Add(EventType.Position, null);
            EventDict.Add(EventType.ServiceTransition, null);
            EventDict.Add(EventType.SessionEnd, null);            
            EventDict.Add(EventType.Status, null);
            EventDict.Add(EventType.Tick, null);
        }

        #endregion

        #region Public Methods

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
        }

        public virtual void Publish(DarkLightEventArgs de)
        {
        }

        #endregion
    }

    //This hub does everything local
    public class LocalHub : Hub
    {
        #region Public Methods

        public override void Publish(DarkLightEventArgs de)
        {
            if (EventDict[de.Type] != null)
                EventDict[de.Type](de);
        }

        #endregion
    }

    //This hub publishes ticks to zeromq
    //Also demonstrates BSON deserialization
    //TODO: serialize TradeLink objects to BSON 
    public class ZeroHub : Hub
    {
        /*
        #region Private Members

        Context _context;
        Socket _publisher;

        #endregion

        #region Public Methods

        public override void Start()
        {
            _publisher = _context.Socket(SocketType.PUB);
            _publisher.Bind("tcp://*:5563");
        }

        public override void Stop()
        {
        }

        public override void Publish(DarkLightEventArgs de)
        {
            //serialize tick to string
            var tickString = TickImpl.Serialize(de.Tick);

            //serialize tick to BSON
            TickString tickStringObj = new TickString(tickString);
            var tickDoc = tickStringObj.ToBsonDocument();
            var packet = tickDoc.ToBson();

            _publisher.SendMore(EventType.Tick);
            _publisher.Send(packet);
        }

        #endregion
        */
    }

    //This hub receives ticks from zeromq and processes locally
    //Also demonstrates BSON serialization
    //TODO: serialize TradeLink objects to BSON
    public class MixedHub : Hub
    {
        /*
        bool _run;
        Context _context;
        List<int> _subscriptions;

        #region Public Methods

        public override void Start()
        {
            Task.Factory.StartNew(RunListener);
        }

        public override void Publish(DarkLightEventArgs de)
        {
            if (EventDict[de.Type] != null)
                EventDict[de.Type](de);
        }

        #endregion

        #region Private Methods

        void RunListener()
        {
            var items = new PollItem[1];
            var subscriber = _context.Socket(SocketType.SUB);
            subscriber.Connect("tcp://localhost:5563");
            subscriber.Subscribe(EventType.Tick);

            items[0] = subscriber.CreatePollItem(IOMultiPlex.POLLIN);
            items[0].PollInHandler += HandlePacket;

            while (_run)
            {
                _context.Poll(items, -1);
            }
        }

        void HandlePacket(Socket socket, IOMultiPlex revents)
        {
            byte[] packetType = socket.Recv();
            byte[] packetData = socket.Recv();

            var tickString = BsonSerializer.Deserialize<TickString>(packetData);
            var tick = TickImpl.Deserialize(tickString.Tick);

            var de = new DarkLightEventArgs(packetType);
            de.Tick = tick;
            EventDict[EventType.Tick](de);
        }
        #endregion
        */
    }

    public class MultiHub : Hub
    {
        //TODO: make hub configurable, able to handle both local and remote messaging for publications and subscriptions
    }

 
}
