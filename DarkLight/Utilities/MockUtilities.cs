using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Common.Models;

namespace DarkLight.Utilities
{
    public static class MockUtilities
    {
        public static List<DarkLightFill> GenerateFills(string backtestid, int numFills)
        {
            var fills = new List<DarkLightFill>();
            Random random = new Random();
            var date = DateTime.Now.AddDays(-20);
            int id = random.Next(10000, 20000);
            double maxprice = 85.0;
            double minprice = 80.0;

            for (int i = 0; i < numFills; i++)
            {
                fills.Add(new DarkLightFill
                              {
                                  Time = date.AddMinutes(i).AddSeconds(random.Next(0, 59)),
                                  Symbol = "IBM: " + backtestid,
                                  Side = random.NextDouble() > 0.5 ? "Long" : "Short",
                                  Size = random.Next(1, 50),
                                  Price = Math.Round(random.NextDouble()*(maxprice - minprice) + minprice, 2),
                                  Id = id++
                              });
            }

            return fills;
        }

        public static List<DarkLightOrder> GenerateOrders(string backtestid, int numFills)
        {
            var orders = new List<DarkLightOrder>();
            Random random = new Random();
            var date = DateTime.Now.AddDays(-20);
            int id = random.Next(10000, 20000);
            double maxprice = 85.0;
            double minprice = 80.0;
            for (int i = 0; i < numFills; i++)
            {
                orders.Add(new DarkLightOrder
                               {
                                   Time = date.AddMinutes(i).AddSeconds(random.Next(0, 59)),
                                   Symbol = "IBM: " + backtestid,
                                   Side = random.NextDouble() > 0.5 ? "Long" : "Short",
                                   Size = random.Next(1, 50),
                                   Price = Math.Round(random.NextDouble()*(maxprice - minprice) + minprice, 2),
                                   Id = id++
                               });
            }

            return orders;
        }

        public static List<DarkLightPosition> GeneratePositions(string backtestid, int numFills)
        {
            var positions = new List<DarkLightPosition>();
            Random random = new Random();
            var date = DateTime.Now.AddDays(-20);
            double maxprice = 85.0;
            double minprice = 80.0;
            for (int i = 0; i < 20; i++)
            {
                positions.Add(new DarkLightPosition
                                  {
                                      Time = date.AddMinutes(i).AddSeconds(random.Next(0, 59)),
                                      Symbol = "IBM: " + backtestid,
                                      Side = random.NextDouble() > 0.5 ? "Long" : "Short",
                                      Size = random.Next(1, 50),
                                      AvgPrice = Math.Round(random.NextDouble()*(maxprice - minprice) + minprice, 2),
                                      Profit = Math.Round(random.NextDouble()*100, 2),
                                      Points = Math.Round(random.NextDouble()*100, 2)
                                  });
            }

            return positions;
        }

        public static List<DarkLightTick> GenerateTicks(string backtestid, int numFills)
        {
            var ticks = new List<DarkLightTick>();
            Random random = new Random();
            var date = DateTime.Now.AddDays(-20);

            for (int i = 0; i < numFills; i++)
            {
                ticks.Add(new DarkLightTick
                              {
                                  Time = date.AddMinutes(i).AddSeconds(random.Next(0, 59)),
                                  Sym = "IBM: " + backtestid,
                                  Trade = random.Next(1, 50),
                                  TSize = random.Next(1, 50),
                                  Bid = random.Next(1, 50),
                                  Ask = random.Next(1, 50),
                                  BSize = random.Next(1, 50),
                                  ASize = random.Next(1, 50),
                                  TExch = "CME",
                                  BidExch = "--",
                                  AskExch = "--",
                              });
            }

            return ticks;
        }

    }
}
