using Grc.ui.App.Http.Endpoints;
using Microsoft.Extensions.Options;

namespace Grc.ui.App.Utils {
    public class EndpointTypeProvider : IEndpointTypeProvider {

        private readonly EndpointTypeOptions _options;
        public SystemAccessEndpoints Sam => _options.Sam;
        public HealthEndpoint Health => _options.Health;
        public RegistrationEndpoints Registration => _options.Registration;
        public ActivityLogEndpoints ActivityLog => _options.ActivityLog;
        public DepartmentEndpoints Departments => _options.Departments;
        public OrganizationEndpoints Organization  => _options.Organization;
        public ErrorEndpoints Error => _options.Errors;

        

        public EndpointTypeProvider(IOptions<EndpointTypeOptions> options) {
            _options = options.Value;  
        }
    }
}
