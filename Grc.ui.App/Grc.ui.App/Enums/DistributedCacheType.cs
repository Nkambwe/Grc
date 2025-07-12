using System.Runtime.Serialization;

namespace Grc.ui.App.Enums {

    /// <summary>
    /// Distributed cache types enumeration
    /// </summary>
    public enum DistributedCacheType {
        [EnumMember(Value = "memory")]
        Memory,
        [EnumMember(Value = "sqlserver")]
        SqlServer,
        [EnumMember(Value = "redis")]
        Redis
    }
}
