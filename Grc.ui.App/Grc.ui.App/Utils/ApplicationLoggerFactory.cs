using Grc.ui.App.Utils;
using Microsoft.Extensions.Options;

public class ApplicationLoggerFactory : IApplicationLoggerFactory {
    private readonly IEnvironmentProvider _environmentProvider;
     private readonly IOptions<LoggingOptions> _loggingOptions;

    public ApplicationLoggerFactory(IEnvironmentProvider environmentProvider, IOptions<LoggingOptions> loggingOptions) {
        _environmentProvider = environmentProvider;
        _loggingOptions = loggingOptions;
    }

    public IApplicationLogger CreateLogger()
        => new ApplicationLogger(_environmentProvider,  _loggingOptions, _loggingOptions.Value.DefaultLogName);

    public IApplicationLogger CreateLogger(string name)
        => new ApplicationLogger(_environmentProvider, _loggingOptions, name);
}