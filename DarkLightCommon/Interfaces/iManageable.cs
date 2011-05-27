using System.Collections.Generic;
using DarkLight.Common.Services;

namespace DarkLight.Common.Interfaces
{
    public class DarkLightProductInfo
    {
        public int DarkLightProductID { get; set; }
        public string Ticker { get; set; }
    }

    public interface iManageable
    {
        void InitializeDependencies(List<string> tickers);
        void InitializeStrategy();
        void StartStrategy();
        void StopStrategy();
        StrategyInfo GetStrategyInfo();
    }
}
