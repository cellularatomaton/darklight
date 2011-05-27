using System.Collections.Generic;

namespace DarkLight.Common.Interfaces
{
    public class BookDepth
    {
        public long ExchangeTime { get; set; }
        public long OurTime { get; set; }
        public int ProductID { get; set; }
        public int ImpliedBidPrice { get; set; }
        public int ImpliedBidQuantity { get; set; }
        public int ImpliedOfferPrice { get; set; }
        public int ImpliedOfferQuantity { get; set; }
        public List<int> BidPrices { get; set; }
        public List<int> BidQuantities { get; set; }
        public List<int> OfferPrices { get; set; }
        public List<int> OfferQuantities { get; set; }
    }

    public class TimeAndSales
    {
        public long ExchangeTime { get; set; }
        public long OurTime { get; set; }
        public int PriceInTicks { get; set; }
        public int Quantity { get; set; }
        public int Volume { get; set; }
    }

    public delegate void TimeAndSalesHandler(TimeAndSales ts);
    public delegate void BookDepthHandler(BookDepth bd);

    public interface iMarketDataProvider
    {
        void Initialize(List<string> tickers);
        void RegisterBookDepthHandler(BookDepthHandler callback);
        void RegisterTimeAndSalesHandler(TimeAndSalesHandler callback);

        void StartHandlingBookDepth();
        void StartHandlingTimeAndSales();
    }
}
