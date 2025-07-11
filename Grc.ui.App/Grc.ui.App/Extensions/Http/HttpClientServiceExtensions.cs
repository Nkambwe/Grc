using Polly;
using Polly.Extensions.Http;
//using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Grc.ui.App.Extensions.Http {
    public static class HttpClientServiceExtensions {

        public static IServiceCollection AddApiClients(this IServiceCollection services, IConfiguration configuration) {

            //..add Polly for resilience patterns
            services.AddHttpClient("PollyWaitAndRetry")
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            //..add the main API clients with service abstractions
            services.AddApiServices(configuration);

            return services;
        }

        private static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration) {
            //var baseUrl = configuration["MiddlewareOptions:BaseUrl"];

            // Register typed HTTP clients for different service areas
            //services.AddHttpClient<ISystemAccessManagementClient, SystemAccessManagementClient>(client =>
            //{
            //    client.BaseAddress = new Uri(baseUrl);
            //    ConfigureClient(client);
            //})
            //.AddPolicyHandler(GetRetryPolicy())
            //.AddPolicyHandler(GetCircuitBreakerPolicy())
            //.AddPolicyHandler(GetTimeoutPolicy());

            //services.AddHttpClient<ICustomerManagementClient, CustomerManagementClient>(client =>{
            //    client.BaseAddress = new Uri(baseUrl);
            //    ConfigureClient(client);
            //})
            //.AddPolicyHandler(GetRetryPolicy())
            //.AddPolicyHandler(GetCircuitBreakerPolicy())
            //.AddPolicyHandler(GetTimeoutPolicy());

            //services.AddHttpClient<IAccountsManagementClient, AccountsManagementClient>(client => {
            //    client.BaseAddress = new Uri(baseUrl);
            //    ConfigureClient(client);
            //})
            //.AddPolicyHandler(GetRetryPolicy())
            //.AddPolicyHandler(GetCircuitBreakerPolicy())
            //.AddPolicyHandler(GetTimeoutPolicy());


            //services.AddHttpClient<ISettingsManagementClient, SettingsManagementClient>(client => {
            //    client.BaseAddress = new Uri(baseUrl);
            //    ConfigureClient(client);
            //})
            //.AddPolicyHandler(GetRetryPolicy())
            //.AddPolicyHandler(GetCircuitBreakerPolicy())
            //.AddPolicyHandler(GetTimeoutPolicy());

            return services;
        }

        private static void ConfigureClient(HttpClient client) {
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromSeconds(30);
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }

    }
}
