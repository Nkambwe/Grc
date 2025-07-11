using Grc.ui.App.Utils;
using System.Text;
using System.Text.Json;

namespace Grc.ui.App.Services {

    public class GrcBaseService : IGrcBaseService {
        protected readonly HttpClient GrcHttpClient;
        protected readonly IApplicationLogger Logger;
        protected readonly IEnvironmentProvider Environment;
        protected readonly IEndpointTypeProvider EndpointType;
        protected readonly JsonSerializerOptions JsonOptions;

        public GrcBaseService(IApplicationLoggerFactory loggerFactory, 
                              IHttpClientFactory httpClientFactory,
                              IEnvironmentProvider environment,
                              IEndpointTypeProvider endpointType) {
            Logger = loggerFactory.CreateLogger("app_services");
            Environment = environment;
            EndpointType = endpointType;
            JsonOptions = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            //..create client
            GrcHttpClient = httpClientFactory.CreateClient("MiddlewareClient");
        }


        public async Task<TResponse> GetAsync<TResponse>(string endpoint) {
            try {
                Logger.LogActivity($"GET Request to: {endpoint}", "INFO");
            
                var response = await GrcHttpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
        
                var content = await response.Content.ReadAsStringAsync();
                Logger.LogActivity($"GET Response received from: {endpoint}", "INFO");
            
                return JsonSerializer.Deserialize<TResponse>(content, JsonOptions);
            } catch (Exception ex) {
                Logger.LogActivity($"GET Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task<TResponse> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data) {
            try {
                Logger.LogActivity($"PATCH Request to: {endpoint}", "INFO");
            
                var jsonContent = JsonSerializer.Serialize(data, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
                var response = await GrcHttpClient.PatchAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
        
                var responseContent = await response.Content.ReadAsStringAsync();
                Logger.LogActivity($"PATCH Response received from: {endpoint}", "INFO");
            
                return JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
            } catch (Exception ex) {
                Logger.LogActivity($"PATCH Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task PatchAsync<TRequest>(string endpoint, TRequest data) {
            try {
                Logger.LogActivity($"PATCH Request (no response) to: {endpoint}", "INFO");
            
                var jsonContent = JsonSerializer.Serialize(data, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
                var response = await GrcHttpClient.PatchAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
            
                Logger.LogActivity($"PATCH Request completed for: {endpoint}", "INFO");
            } catch (Exception ex) {
                Logger.LogActivity($"PATCH Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data) {
            try {
                Logger.LogActivity($"POST Request to: {endpoint}", "INFO");
            
                var jsonContent = JsonSerializer.Serialize(data, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
                var response = await GrcHttpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
        
                var responseContent = await response.Content.ReadAsStringAsync();
                Logger.LogActivity($"POST Response received from: {endpoint}", "INFO");
            
                return JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
            }  catch (Exception ex) {
                Logger.LogActivity($"POST Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task PostAsync<TRequest>(string endpoint, TRequest data) {
            try {
                Logger.LogActivity($"POST Request (no response) to: {endpoint}", "INFO");
            
                var jsonContent = JsonSerializer.Serialize(data, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
                var response = await GrcHttpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
            
                Logger.LogActivity($"POST Request completed for: {endpoint}", "INFO");
            } catch (Exception ex) {
                Logger.LogActivity($"POST Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data) {
            try {
                Logger.LogActivity($"PUT Request to: {endpoint}", "INFO");
            
                var jsonContent = JsonSerializer.Serialize(data, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
                var response = await GrcHttpClient.PutAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
        
                var responseContent = await response.Content.ReadAsStringAsync();
                Logger.LogActivity($"PUT Response received from: {endpoint}", "INFO");
            
                return JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
            } catch (Exception ex) {
                Logger.LogActivity($"PUT Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task PutAsync<TRequest>(string endpoint, TRequest data) {
            try {
                Logger.LogActivity($"PUT Request (no response) to: {endpoint}", "INFO");
            
                var jsonContent = JsonSerializer.Serialize(data, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
                var response = await GrcHttpClient.PutAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
            
                Logger.LogActivity($"PUT Request completed for: {endpoint}", "INFO");
            } catch (Exception ex) {
                Logger.LogActivity($"PUT Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task<TResponse> SendAsync<TResponse>(HttpMethod method, string endpoint, object requestBody = null) {
            try  {
                Logger.LogActivity($"{method.Method} Request to: {endpoint}", "INFO");
            
                var request = new HttpRequestMessage(method, endpoint);
                if (requestBody != null) {
                    var jsonContent = JsonSerializer.Serialize(requestBody, JsonOptions);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }
            
                var response = await GrcHttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
        
                var responseContent = await response.Content.ReadAsStringAsync();
                Logger.LogActivity($"{method.Method} Response received from: {endpoint}", "INFO");
            
                return JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
            }  catch (Exception ex) {
                Logger.LogActivity($"{method.Method} Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task SendAsync(HttpMethod method, string endpoint, object requestBody = null) {
            try  {
                Logger.LogActivity($"{method.Method} Request (no response) to: {endpoint}", "INFO");
                var request = new HttpRequestMessage(method, endpoint);
            
                if (requestBody != null) {
                    var jsonContent = JsonSerializer.Serialize(requestBody, JsonOptions);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }
            
                var response = await GrcHttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            
                Logger.LogActivity($"{method.Method} Request completed for: {endpoint}", "INFO");
            } catch (Exception ex)  {
                Logger.LogActivity($"{method.Method} Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }
        
        public async Task<TResponse> DeleteAsync<TResponse>(string endpoint) {
            try {
                Logger.LogActivity($"DELETE Request to: {endpoint}", "INFO");
            
                var response = await GrcHttpClient.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();
        
                var responseContent = await response.Content.ReadAsStringAsync();
                Logger.LogActivity($"DELETE Response received from: {endpoint}", "INFO");
            
                return JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
            } catch (Exception ex) {
                Logger.LogActivity($"DELETE Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task DeleteAsync(string endpoint) {
            try {
                Logger.LogActivity($"DELETE Request (no response) to: {endpoint}", "INFO");
            
                var response = await GrcHttpClient.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();
            
                Logger.LogActivity($"DELETE Request completed for: {endpoint}", "INFO");
            } catch (Exception ex) {
                Logger.LogActivity($"DELETE Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }
    }

}
