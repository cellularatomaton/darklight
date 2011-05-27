#define LOGGING

using System;
using System.Collections.Generic;
using DarkLight.Common.Interfaces;
using DarkLight.Common.Services;

namespace DarkLight.Common.Strategies
{
    public class TestStrategy : iManageable
    {
        private PlaceNewOrderHandler PlaceNewOrder;
        private ModifyOrderHandler ModifyOrder;
        private CancelOrderHandler CancelOrder;

        private iMarketDataProvider marketDataClient;
        private iOrderRoutingProvider orderRoutingClient;
        private iPositionProvider positionClient;

        private bool trading = false;
        private StrategyInfo stratInfo;

//#if LOGGING
//        private CSVLogger priceLogger;
//        private CSVLogger bookLogger;
//        private CSVLogger orderLogger;
//        private CSVLogger positionLogger;
//#endif

        public TestStrategy(
            StrategyInfo info,
            iMarketDataProvider marketDataProvider,
            iOrderRoutingProvider orderRoutingProvider,
            iPositionProvider positionProvider)
        {
//#if LOGGING
//            priceLogger = new CSVLogger(@"F:\Logs\TestStrategyPriceLog.csv", -1);
//            bookLogger = new CSVLogger(@"F:\Logs\TestStrategyBookLog.csv", -1);
//            orderLogger = new CSVLogger(@"F:\Logs\TestStrategyOrderLog.csv", -1);
//            positionLogger = new CSVLogger(@"F:\Logs\TestStrategyPositionLog.csv", -1);
//#endif
            stratInfo = info;
            marketDataClient = marketDataProvider;
            orderRoutingClient = orderRoutingProvider;
            positionClient = positionProvider;
        }

        protected void HandleNewAck(OrderAcknowledgement ack)
        {
#if LOGGING
            LogAck(ack);
#endif
        }

        protected void HandleModifyAck(OrderAcknowledgement ack)
        {
#if LOGGING
            LogAck(ack);
#endif
        }

        protected void HandleCancelAck(OrderAcknowledgement ack)
        {
#if LOGGING
            LogAck(ack);
#endif
        }

        protected void HandleFillAck(OrderAcknowledgement ack)
        {
#if LOGGING
            LogAck(ack);
#endif
        }

        protected void LogAck(OrderAcknowledgement ack)
        {
            //orderLogger.LogEvent(
            //    ack.AcknowledgementType + "," +
            //    ack.OrderStatus + "," +
            //    ack.FillStatus + "," +
            //    ack.PriceInTicks + "," +
            //    ack.InitialQuantity + "," +
            //    ack.RemainingQuantity + "," +
            //    ack.FilledQuantity);
        }

        protected void HandleBookDepth(BookDepth bookDepth)
        {
//#if LOGGING
//            bookLogger.LogEvent(
//                bd.iProductId + "," +
//                bd.book.bidPrice[1] + "," +
//                bd.book.bidSize[1] + "," +
//                bd.book.offerSize[1] + "," +
//                bd.book.offerPrice[1]);
//#endif
            WorkOrders(bookDepth);
        }

        protected void HandleTimeAndSales(TimeAndSales timeAndSales)
        {
//#if LOGGING
//            priceLogger.LogEvent(
//                pu.iProductId + "," +
//                pu.price.iPriceInTicks + "," +
//                pu.price.iSize);
//#endif
        }

        protected void HandlePositionUpdate(int position, int productIndex)
        {
//#if LOGGING
//            positionLogger.LogEvent(
//                position + "," +
//                productIndex);
//#endif
        }

        private Dictionary<int, DarkLightProductInfo> productInfoByProductID;
        private Dictionary<int, bool> bidInMarketByProductID;
        private Dictionary<int, bool> offerInMarketByProductID;
        private int bidSize = 1000;
        private int offerSize = -1000;
        private Dictionary<int, int> bidLevelByProductID;
        private Dictionary<int, int> offerLevelByProductID;
        private Dictionary<int, int> bidOrderIDByProductID;
        private Dictionary<int, int> offerOrderIDByProductID;
        /// <summary>
        /// This method is simply meant to test order functionality.
        /// </summary>
        /// <param name="b"></param>
        private void WorkOrders(BookDepth bd)
        {
            if (trading)
            {
                var productID = bd.ProductID;
                
                var bestBidLevel = bd.BidPrices[0];
                var bestOfferLevel = bd.OfferPrices[0];
                var bidLevel = bestBidLevel - 5;
                var offerLevel = bestOfferLevel + 5;

                if (bidLevelByProductID[productID] != bidLevel)
                {
                    if (bidInMarketByProductID[productID])
                    {
                        var orderID = bidOrderIDByProductID[productID];
                        ModifyOrder(productID, orderID, bidLevel, bidSize, bidSize);
                    }
                    else
                    {
                        var orderID = PlaceNewOrder(productID, bidLevel, bidSize);
                        bidInMarketByProductID[productID] = true;
                        bidOrderIDByProductID[productID] = orderID;
                    }
                    bidLevelByProductID[productID] = bidLevel;
                }

                if (offerLevelByProductID[productID] != offerLevel)
                {
                    if (offerInMarketByProductID[productID])
                    {
                        var orderID = offerOrderIDByProductID[productID];
                        ModifyOrder(productID, orderID, offerLevel, offerSize, offerSize);
                    }
                    else
                    {
                        var orderID = PlaceNewOrder(productID, offerLevel, offerSize);
                        offerInMarketByProductID[productID] = true;
                        offerOrderIDByProductID[productID] = orderID;
                    }
                    offerLevelByProductID[productID] = offerLevel;
                }
            }
        }

        private void CancelAllOrders()
        {
            foreach (var prodID in bidOrderIDByProductID.Keys)
            {
                var orderID = bidOrderIDByProductID[prodID];
                if (orderID != -1)
                {
                    CancelOrder(orderID);
                    bidInMarketByProductID[prodID] = false;
                }
            }
            foreach (var prodID in offerOrderIDByProductID.Keys)
            {
                var orderID = offerOrderIDByProductID[prodID];
                if (orderID != -1)
                {
                    CancelOrder(orderID);
                    offerInMarketByProductID[prodID] = false;
                }
            }
        }

        private void GetReadyToTrade()
        {
            productInfoByProductID = orderRoutingClient.GetDarkLightProductInfos();
            bidInMarketByProductID = new Dictionary<int, bool>();
            offerInMarketByProductID = new Dictionary<int, bool>();
            bidLevelByProductID = new Dictionary<int, int>();
            offerLevelByProductID = new Dictionary<int, int>();
            bidOrderIDByProductID = new Dictionary<int, int>();
            offerOrderIDByProductID = new Dictionary<int, int>();

            foreach (var key in productInfoByProductID.Keys)
            {
                bidInMarketByProductID[key] = false;
                offerInMarketByProductID[key] = false;
                bidLevelByProductID[key] = Int32.MinValue;
                offerLevelByProductID[key] = Int32.MaxValue;
                bidOrderIDByProductID[key] = -1;
                offerOrderIDByProductID[key] = -1;
            }
        }

        public void InitializeDependencies(List<string> tickers)
        {
            orderRoutingClient.Initialize(tickers);
            marketDataClient.Initialize(tickers);
            positionClient.Initialize(tickers);
        }

        public void InitializeStrategy()
        {
            marketDataClient.RegisterTimeAndSalesHandler(HandleTimeAndSales);
            marketDataClient.RegisterBookDepthHandler(HandleBookDepth);

            positionClient.RegisterPositionUpdateHandler(HandlePositionUpdate);

            orderRoutingClient.RegisterAckHandlers(
                HandleNewAck,
                HandleModifyAck,
                HandleCancelAck,
                HandleFillAck);

            PlaceNewOrder = orderRoutingClient.GetPlaceNewOrderHandler();
            ModifyOrder = orderRoutingClient.GetModifyOrderHandler();
            CancelOrder = orderRoutingClient.GetCancelOrderhandler();

            GetReadyToTrade();
        }

        public void StartStrategy()
        {
            trading = true;
            positionClient.StartHandlingPositionUpdates();
            orderRoutingClient.StartHandlingAcks();
            marketDataClient.StartHandlingTimeAndSales();
            marketDataClient.StartHandlingBookDepth();
            stratInfo.Status = StrategyStatus.STARTED;
        }

        public void StopStrategy()
        {
            trading = false;
            CancelAllOrders();
            stratInfo.Status = StrategyStatus.STOPPED;
        }

        public StrategyInfo GetStrategyInfo()
        {
            return stratInfo;
        }
    }
}
