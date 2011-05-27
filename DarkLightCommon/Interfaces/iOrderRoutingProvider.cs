using System.Collections.Generic;

namespace DarkLight.Common.Interfaces
{
    public enum AckType
    {
        NEW,
        MODIFY,
        CANCEL
    }

    public enum OrderState
    {
        RESTING,
        PENDING,
        REJECTED
    }

    public enum FillState
    {
        UNFILLED,
        PARTIALLY_FILLED,
        FILLED
    }

    public class OrderAcknowledgement
    {
        public AckType AcknowledgementType { get; set; }
        public OrderState OrderStatus { get; set; }
        public FillState FillStatus { get; set; }
        public int PriceInTicks { get; set; }
        public int InitialQuantity { get; set; }
        public int FilledQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public int ProductID { get; set; }
        public string Ticker { get; set; }
    }

    public delegate void OrderAckHandler(OrderAcknowledgement ack);
    public delegate int PlaceNewOrderHandler(int productInfoEntityID, int price, int quantity);
    public delegate void ModifyOrderHandler(int productInfoEntityID,int strategyOrderID, int price, int initQuantity, int remQuantity);
    public delegate void CancelOrderHandler(int strategyOrderID);

    public interface iOrderRoutingProvider
    {
        Dictionary<int, DarkLightProductInfo> GetDarkLightProductInfos();
        void Initialize(List<string> tickers);

        void RegisterAckHandlers(
            OrderAckHandler newCallback,
            OrderAckHandler modifyCallback,
            OrderAckHandler cancelCallback,
            OrderAckHandler fillCallback);

        void StartHandlingAcks();
        void StopHandlingAcks();

        PlaceNewOrderHandler GetPlaceNewOrderHandler();
        ModifyOrderHandler GetModifyOrderHandler();
        CancelOrderHandler GetCancelOrderhandler();
    }
}
