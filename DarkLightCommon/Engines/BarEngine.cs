using System.Collections.Generic;
using System.Linq;
using DarkLight.Common.Interfaces;

namespace DarkLight.Common.Engines
{
    public delegate void PriceBarHandler(PriceBar pb);
    public delegate void VolumeBarHandler(VolumeBar vb);

    public class PriceBar
    {
        public int Open { get; set; }
        public int High { get; set; }
        public int Low { get; set; }
        public int Close { get; set; }
    }

    public class VolumeBar
    {
        public int Total { get; set; }
        public int Average { get; set; }
    }

    public class BarEngine
    {
        private iMarketDataProvider marketDataClient;
        private Queue<TimeAndSales> prices;
        private long intervalStart;
        private long intervalEnd;
        private long intervalSize;
        private bool startTimeNeeded = true;
        private PriceBarHandler HandlePriceBar;

        public BarEngine(
            iMarketDataProvider dataProvider,
            long intervalSizeInMilliseconds)
        {
            marketDataClient = dataProvider;
            intervalSize = intervalSizeInMilliseconds;
            intervalStart = long.MinValue;
            intervalEnd = long.MinValue + intervalSize;
            prices = new Queue<TimeAndSales>();
        }

        public void RegisterPriceBarHandler(PriceBarHandler callback)
        {
            HandlePriceBar += callback;
        }

        public void StartHandlingPriceUpdates()
        {
            marketDataClient.RegisterTimeAndSalesHandler(HandleTimeAndSales);
            marketDataClient.StartHandlingTimeAndSales();
        }

        private void HandleTimeAndSales(TimeAndSales ts)
        {
            if(startTimeNeeded)
            {
                intervalStart = ts.OurTime;
                intervalEnd = intervalStart + intervalSize;
                startTimeNeeded = false;
            }

            if(intervalStart < ts.OurTime &&
                ts.OurTime < intervalEnd)
            {
                prices.Enqueue(ts);
            }
            else
            {
                HandlePriceBar(GetBarFromQueue(prices));
                prices.Clear();
                prices.Enqueue(ts);
            }
        }

        private PriceBar GetBarFromQueue(IEnumerable<TimeAndSales> tsWindow)
        {
            return new PriceBar
                       {
                           Open = tsWindow.First().PriceInTicks,
                           High = tsWindow.Max(p => p.PriceInTicks),
                           Low = tsWindow.Min(p => p.PriceInTicks),
                           Close = tsWindow.Last().PriceInTicks
                       };
        }
    }
}
