using System.Runtime.Serialization;

namespace DarkLight.Common.Services
{
    [DataContract]
    public enum StrategyStatus
    {
        [EnumMember]
        STARTED,
        [EnumMember]
        STOPPED
    }

    [DataContract]
    public class StrategyInfo
    {
        [DataMember]
        public string StrategyName { get; set; }

        [DataMember]
        public StrategyStatus Status { get; set; }
    }
}
