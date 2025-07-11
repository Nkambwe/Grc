using Grc.ui.App.Http.Endpoints;

namespace Grc.ui.App.Utils {
    public interface IEndpointTypeProvider {
        SystemAccessEndpoints Sam { get;}
        HealthEndpoint Health { get; }
    }   
}
