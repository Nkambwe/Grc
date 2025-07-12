using Grc.ui.App.Infrastructure.AppSettings;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace Grc.ui.App.Infrastructure {

    public class AppSettingParameters {
        /// <summary>
        /// Gets or sets cache settings parameters
        /// </summary>
        public CacheSettings CacheSettings { get; set; } = new CacheSettings();

        /// <summary>
        /// Gets or sets distributed cache settings parameters
        /// </summary>
        public DistributedCacheSettings DistributedCacheSettings { get; set; } = new DistributedCacheSettings();

        /// <summary>
        /// Get or Set general application settings
        /// </summary>
        public GeneralSettings GeneralSettings { get; set; } = new GeneralSettings();

        /// <summary>
        /// Gets or sets additional configuration parameters
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalSettings { get; set; }
    }
}
