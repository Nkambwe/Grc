namespace Grc.Middleware.Api.Http.Responses {
    public class ConfigurationParameterResponse<T> {
        public string ParameterName { get; set; }
        public string ParameterType { get; set; }
        public T Value { get; set; }
    }
}
