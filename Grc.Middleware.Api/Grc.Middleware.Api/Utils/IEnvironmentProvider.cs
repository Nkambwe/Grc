namespace Grc.Middleware.Api.Utils { 

    public interface IEnvironmentProvider {
        bool IsLive { get; }
    }

}
