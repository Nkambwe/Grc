
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Grc.ui.App.Services {

    public class MiddlewareHealthService : IMiddlewareHealthService {

        private readonly IEndpointTypeProvider _endpointProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEnvironmentProvider _environment;
        private readonly IApplicationLogger _logger;
        public MiddlewareHealthService(IApplicationLoggerFactory loggerFactory, 
                                       IHttpClientFactory httpClientFactory,
                                       IEndpointTypeProvider endpointProvider,
                                       IOptions<MiddlewareOptions> middlewareOptions,
                                       IEnvironmentProvider environment) {
            _endpointProvider = endpointProvider;
            _httpClientFactory = httpClientFactory;
            _environment = environment;
            _logger = loggerFactory.CreateLogger("app_services");
            _logger.Channel = $"HEALTH-CHECK-{DateTime.Now:yyyyMMddHHmmss}";
        }

        public async Task<(bool status, bool isConnected, bool hasCompanie)> CheckMiddlewareStatusAsync() {
            try {

                //..build endpoint
                var endpoint = $"{_endpointProvider.Health.Status}";
                _logger.LogActivity($"Endpoint: {endpoint}");
                //.. get configured HttpClient
                var httpClient = _httpClientFactory.CreateClient("MiddlewareClient");
                var fullUrl = $"{httpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                _logger.LogActivity($"MIDDLEWARE URL: {fullUrl}");

                //..send request
                var response = await httpClient.GetAsync(endpoint);
                _logger.LogActivity($"Response Status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode) {
                    _logger.LogActivity($"Middleware call failed with status: {response.StatusCode}", "WARNING");
                    return (false, false, false);
                }
        
                //..read and deserialize response
                var responseData = await response.Content.ReadAsStringAsync();
                _logger.LogActivity($"Response Data: {responseData}");
        
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<HealthCheckModel>(responseData, options);
        
                if (result == null) {
                    _logger.LogActivity("Failed to deserialize response", "ERROR");
                    return (false, false, false);
                }
        
                _logger.LogActivity($"Health Check Result - Status: {result.Status}, Connected: {result.IsConnected}, HasCompany: {result.HasCompany}");
                return (result.Status, result.IsConnected, result.HasCompany);
            } catch (HttpRequestException httpEx) {
                _logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                _logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                return (false, false, false);
            } catch (TaskCanceledException timeoutEx) when (timeoutEx.InnerException is TimeoutException) {
                _logger.LogActivity("Request timeout", "ERROR");
                _logger.LogActivity(timeoutEx.StackTrace, "STACKTRACE");
                return (false, false, false);
            } catch (JsonException jsonEx) {
                _logger.LogActivity($"JSON Deserialization Error: {jsonEx.Message}", "ERROR");
                _logger.LogActivity(jsonEx.StackTrace, "STACKTRACE");
                return (false, false, false);
            } catch (Exception ex)  {
                _logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                _logger.LogActivity(ex.StackTrace, "STACKTRACE");
                return (false, false, false);
            }
        }

    }
}
