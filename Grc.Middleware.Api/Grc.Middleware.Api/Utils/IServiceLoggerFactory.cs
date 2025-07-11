namespace Grc.Middleware.Api.Utils { 

    public interface IServiceLoggerFactory {
        IServiceLogger CreateLogger();
        IServiceLogger CreateLogger(string name);
    }
    
}


