namespace Grc.ui.App.Infrastructure.AppSettings {

    /// <summary>
    /// Cache configuration parameters for appSettings.json file
    /// </summary>
    public class CacheSettings {
        /// <summary>
        /// Gets or sets the default cache time in minutes
        /// </summary>
        public int DefaultCacheTime { get; set; } = 60;

        /// <summary>
        /// Gets or sets the short term cache time in minutes
        /// </summary>
        public int ShortTermCacheTime { get; set; } = 3;

        /// <summary>
        /// Gets or sets the bundled files cache time in minutes
        /// </summary>
        public int BundledFilesCacheTime { get; set; } = 120;
    }
}
