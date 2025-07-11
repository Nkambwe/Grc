namespace Grc.Middleware.Api.Utils {

    public class LoggingOptions {

        public const string SectionName = "LoggingOptions";
        public string DefaultLogName { get; set; } = "grc_middleware_log";
        public string LogFolder { get; set; } = "grc_middleware";
    }

}
