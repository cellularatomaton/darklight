using System.Collections.Generic;
using DarkLight.Common.Interfaces;
using DarkLight.Common.Services;

namespace DarkLight.Common.Engines
{
    public sealed class StrategyManagementEngine
    {
        private static readonly StrategyManagementEngine instance = new StrategyManagementEngine();
        private Dictionary<int, iManageable> strategies;
        private Dictionary<int, StrategyInfo> strategyInfos;

        public static StrategyManagementEngine Instance
        {
            get { return instance; }
        }
        private StrategyManagementEngine()
        {
            strategies = new Dictionary<int, iManageable>();
            strategyInfos = new Dictionary<int,StrategyInfo>();
        }

        public void AddStrategy(iManageable strat)
        {
            var stratInfo = strat.GetStrategyInfo();
            strategies.Add(stratInfo.StrategyID, strat);
            strategyInfos.Add(stratInfo.StrategyID, stratInfo);
            // Control initialization from here...
            strat.InitializeStrategy();
        }

        public void StartAllStrategies()
        {
            foreach (var managedStrategy in strategies.Values)
            {
                managedStrategy.StartStrategy();
            }
        }

        public void StopAllStrategies()
        {
            foreach (var managedStrategy in strategies.Values)
            {
                managedStrategy.StopStrategy();
            }
        }

        public IEnumerable<StrategyInfo> GetAllStrategyInfoForServer()
        {
            return strategyInfos.Values;
        }

        public void StartStrategy(StrategyInfo strategyInfo)
        {
            strategies[strategyInfo.StrategyID].StartStrategy();
        }

        public void StopStrategy(StrategyInfo strategyInfo)
        {
            strategies[strategyInfo.StrategyID].StopStrategy();
        }
    }
}
