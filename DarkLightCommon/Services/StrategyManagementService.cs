using System.Collections.Generic;
using DarkLight.Common.Engines;

namespace DarkLight.Common.Services
{
    public class StrategyManagementService : iStrategyManagementService
    {
        #region Implementation of iStrategyManagementService

        public IEnumerable<StrategyInfo> GetAllStrategyInfoForServer()
        {
            return StrategyManagementEngine.Instance.GetAllStrategyInfoForServer();
        }

        public void StartStrategy(StrategyInfo strategyInfo)
        {
            StrategyManagementEngine.Instance.StartStrategy(strategyInfo);
        }

        public void StopStrategy(StrategyInfo strategyInfo)
        {
            StrategyManagementEngine.Instance.StopStrategy(strategyInfo);
        }

        #endregion
    }
}
