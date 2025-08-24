namespace Grc.ui.App.Http.Endpoints {
    public class EndpointTypeOptions {

        public const string SectionName = "EndpointTypeOptions";
        public SystemAccessEndpoints Sam { get; set; } = new();
        public HealthEndpoint Health { get; set; } = new();
        public RegistrationEndpoints Registration { get; set; } = new();
        public ErrorEndpoints Errors { get; set; } = new();
    }
}
