using Grc.ui.App.Enums;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Grc.ui.App.Infrastructure.AppSettings {

    public class DistributedCacheSettings {

        /// <summary>
        /// Gets or sets a distributed cache type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DistributedCacheType DistributedCacheType { get; set; } = DistributedCacheType.SqlServer;

        /// <summary>
        /// Gets or sets a value indicating whether we should use distributed cache
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Gets or sets connection string. Used when distributed cache is enabled
        /// </summary>
        public string ConnectionString { get; set; } = "127.0.0.1:6379,ssl=False";

        /// <summary>
        /// Gets or sets schema name. Used when distributed cache is enabled and DistributedCacheType property is set as SqlServer
        /// </summary>
        public string SchemaName { get; set; } = "dbo";

        /// <summary>
        /// Gets or sets table name. Used when distributed cache is enabled and DistributedCacheType property is set as SqlServer
        /// </summary>
        public string TableName { get; set; } = "DistributedCache";
    }
}
