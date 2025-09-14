using Grc.ui.App.Http.Endpoints;

namespace Grc.ui.App.Utils {
    public interface IEndpointTypeProvider {
        SystemAccessEndpoints Sam { get;}
        HealthEndpoint Health { get; }
        RegistrationEndpoints Registration { get; }
        ActivityLogEndpoints ActivityLog { get; }
        DepartmentEndpoints Departments { get; }
        OrganizationEndpoints Organization { get; }
        ErrorEndpoints Error { get; }
    }   
}
