using System.Collections.Generic;
using System.ServiceModel;

namespace DarkLight.Common.Services
{
    [ServiceContract]
    public interface iStrategyManagementService
    {
        [OperationContract]
        IEnumerable<StrategyInfo> GetAllStrategyInfoForServer();

        [OperationContract]
        void StartStrategy(StrategyInfo strategyInfo);

        [OperationContract]
        void StopStrategy(StrategyInfo strategyInfo);
    }

}
