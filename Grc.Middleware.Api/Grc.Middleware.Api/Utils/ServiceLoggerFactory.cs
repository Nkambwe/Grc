using Microsoft.Extensions.Options;

namespace Grc.Middleware.Api.Utils { 
    
    public class ServiceLoggerFactory : IServiceLoggerFactory {
        private readonly IEnvironmentProvider _environmentProvider;
         private readonly IOptions<LoggingOptions> _loggingOptions;

        public ServiceLoggerFactory(IEnvironmentProvider environmentProvider, IOptions<LoggingOptions> loggingOptions) {
            _environmentProvider = environmentProvider;
            _loggingOptions = loggingOptions;
        }

        public IServiceLogger CreateLogger()
            => new ServiceLogger(_environmentProvider,  _loggingOptions, _loggingOptions.Value.DefaultLogName);

        public IServiceLogger CreateLogger(string name)
            => new ServiceLogger(_environmentProvider, _loggingOptions, name);
    }
}
