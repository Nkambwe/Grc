using Grc.ui.App.Http.Endpoints;
using Microsoft.Extensions.Options;

namespace Grc.ui.App.Utils {
    public class EndpointTypeProvider : IEndpointTypeProvider {

        private readonly EndpointTypeOptions _options;
        public SystemAccessEndpoints Sam => _options.Sam;
        public HealthEndpoint Health => _options.Health;
        public RegistrationEndpoints Registration => _options.Registration;
        public ErrorEndpoints Error => _options.Errors;
        public EndpointTypeProvider(IOptions<EndpointTypeOptions> options) {
            _options = options.Value;  
        }
    }
}
