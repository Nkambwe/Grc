using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Utils;
using System.Text;
using System.Text.Json;

namespace Grc.ui.App.Services {

    public class HttpHandler : IHttpHandler {
        protected readonly HttpClient GrcHttpClient;
        protected readonly IApplicationLogger Logger;
        protected readonly JsonSerializerOptions JsonOptions;
        protected readonly IMapper Mapper;

        public HttpHandler(IApplicationLoggerFactory loggerFactory, IHttpClientFactory httpClientFactory) {
            Logger = loggerFactory.CreateLogger("app_services");
            JsonOptions = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            //..create client
            GrcHttpClient = httpClientFactory.CreateClient("MiddlewareClient");
        }

        
        public async Task<GrcResponse<TResponse>> GetAsync<TResponse>(string endpoint) where TResponse : class {
            try {
                Logger.LogActivity($"GRC GET Request to: {endpoint}", "INFO");
                
                //..formulate URL
                var fullUrl = $"{GrcHttpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                Logger.LogActivity($"MIDDLEWARE URL: {fullUrl}");

                //..send request
                var response = await GrcHttpClient.GetAsync(endpoint);
                if(response == null) { 
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADGATEWAY,
                        "Bad Gateway or possible timeout",
                        "The middleware service did not respond or service timeout occurred"
                    );
            
                    Logger.LogActivity($"BAD GATEWAY: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<TResponse>(error);
                }

                //..we received a response
                Logger.LogActivity($"Response Status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode) {
                    Logger.LogActivity($"Middleware call failed with status: {(int)response.StatusCode}", "WARNING");
            
                    var error = new GrcResponseError(
                        (GrcStatusCodes)(int)response.StatusCode,
                        "Could not complete request. An Error occurred",
                        $"HTTP Status Code: {response.StatusCode}"
                    );
            
                    Logger.LogActivity($"SERVICE ERROR: {JsonSerializer.Serialize(error)}");

                    return new GrcResponse<TResponse>(error);
                }
        
                //..read and deserialize response
                var responseData = await response.Content.ReadAsStringAsync();
                Logger.LogActivity($"GRC GET Response received from: {endpoint}", "INFO");
                Logger.LogActivity($"GRC Midleware data : {responseData}");

                try {

                    Logger.LogActivity("Starting deserialization...");
                    var result = JsonSerializer.Deserialize<GrcResponse<TResponse>>(responseData, JsonOptions);
    
                    if (result == null) {
                        Logger.LogActivity("Deserialization returned null", "ERROR");
                        var error = new GrcResponseError(
                            GrcStatusCodes.SERVERERROR,
                            "System Data Error",
                            "Deserialization returned null"
                        );
                        return new GrcResponse<TResponse>(error);
                    }
    
                    Logger.LogActivity($"Deserialization successful. HasError: {result.HasError}");
                    Logger.LogActivity($"SERVICE RESULT: {JsonSerializer.Serialize(result, JsonOptions)}");
                    return result;
                } catch (JsonException jex) {
                    Logger.LogActivity($"Deserialization Failed: {jex.Message}", "ERROR");
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Failed to deserialize response. An error has occurred"
                    );
                    return new GrcResponse<TResponse>(error);
                } catch (Exception ex) { 
                    Logger.LogActivity($"Unexpected deserialization error: {ex.Message}", "ERROR");
                    Logger.LogActivity($"Exception Type: {ex.GetType().Name}", "ERROR");
                    Logger.LogActivity($"StackTrace: {ex.StackTrace}", "ERROR");
    
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Unexpected deserialization error: {ex.Message}"
                    );
                    return new GrcResponse<TResponse>(error);
                }
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (TaskCanceledException timeoutEx) when (timeoutEx.InnerException is TimeoutException) {
                Logger.LogActivity("Request timeout", "ERROR");
                Logger.LogActivity(timeoutEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.TIMEOUT,
                    "Request timeout",
                    "The request took too long to complete"
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (JsonException jsonEx) {
                Logger.LogActivity($"JSON Deserialization Error: {jsonEx.Message}", "ERROR");
                Logger.LogActivity(jsonEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Ooops! Sorry, something went wrong",
                    "Data format error. Could not format data"
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (GRCException ex)  {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<TResponse>(error);
            }
        }

        public async Task<GrcResponse<TResponse>> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data) where TResponse : class{
            try {
                Logger.LogActivity($"GRC PATCH Request to: {endpoint}", "INFO");
                
                Logger.LogActivity($"REQUEST MAP: {JsonSerializer.Serialize(data)}");
                var jsonContent = JsonSerializer.Serialize(data, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                //..requestUrl URL
                var requestUrl = $"{GrcHttpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                Logger.LogActivity($"REQUEST URL: {requestUrl}");

                //..send request
                var response = await GrcHttpClient.PatchAsync(endpoint, content);
                if(response == null) { 
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADGATEWAY,
                        "Bad Gateway or possible timeout",
                        "The middleware service did not respond or service timeout occurred"
                    );
            
                    Logger.LogActivity($"BAD GATEWAY: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<TResponse>(error);
                }

                Logger.LogActivity($"Response Status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode) {
                    Logger.LogActivity($"Middleware call failed with status: {(int)response.StatusCode}", "WARNING");
            
                    var error = new GrcResponseError(
                        (GrcStatusCodes)(int)response.StatusCode,
                        "Could not complete registration. An Error occurred",
                        $"HTTP Status Code: {response.StatusCode}"
                    );
            
                    Logger.LogActivity($"SERVICE ERROR: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<TResponse>(error);
                }
        
                //..read and deserialize response
                var responseData = await response.Content.ReadAsStringAsync();
                Logger.LogActivity($"GRC PATCH Response received from: {endpoint}", "INFO");
                try {
                    var options = new JsonSerializerOptions { 
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true
                    };
    
                    Logger.LogActivity("Starting deserialization...");
                    var result = JsonSerializer.Deserialize<GrcResponse<TResponse>>(responseData, options);
    
                    if (result == null) {
                        Logger.LogActivity("Deserialization returned null", "ERROR");
                        var error = new GrcResponseError(
                            GrcStatusCodes.SERVERERROR,
                            "System Data Error",
                            "Deserialization returned null"
                        );
                        return new GrcResponse<TResponse>(error);
                    }
    
                    Logger.LogActivity($"Deserialization successful. HasError: {result.HasError}");
                    Logger.LogActivity($"SERVICE RESULT: {JsonSerializer.Serialize(result, options)}");
                    return result;
                } catch (JsonException jex) {
                    Logger.LogActivity($"Deserialization Failed: {jex.Message}", "ERROR");
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Failed to deserialize response. An error has occurred"
                    );
                    return new GrcResponse<TResponse>(error);
                } catch (Exception ex) { 
                    Logger.LogActivity($"Unexpected deserialization error: {ex.Message}", "ERROR");
                    Logger.LogActivity($"Exception Type: {ex.GetType().Name}", "ERROR");
                    Logger.LogActivity($"StackTrace: {ex.StackTrace}", "ERROR");
    
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Unexpected deserialization error: {ex.Message}"
                    );
                    return new GrcResponse<TResponse>(error);
                }
           
            } catch (Exception ex) {
                Logger.LogActivity($"GRC PATCH Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task PatchAsync<TRequest>(string endpoint, TRequest data) {
            try {
                Logger.LogActivity($"GRC PATCH Request (no response) to: {endpoint}", "INFO");
                Logger.LogActivity($"REQUEST MAP: {JsonSerializer.Serialize(data)}");

                var jsonContent = JsonSerializer.Serialize(data, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var requestUrl = $"{GrcHttpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                Logger.LogActivity($"REQUEST URL: {requestUrl}");

                var response = await GrcHttpClient.PatchAsync(endpoint, content);

                if (response == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADGATEWAY,
                        "Bad Gateway or possible timeout",
                        "The middleware service did not respond or service timeout occurred"
                    );
                    Logger.LogActivity($"BAD GATEWAY: {JsonSerializer.Serialize(error)}", "ERROR");
                    throw new HttpRequestException(error.ToString());
                }

                Logger.LogActivity($"Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode) {
                    Logger.LogActivity($"Middleware call failed with status: {(int)response.StatusCode}", "WARNING");

                    var error = new GrcResponseError(
                        (GrcStatusCodes)(int)response.StatusCode,
                        "Could not complete PATCH request. An error occurred",
                        $"HTTP Status Code: {response.StatusCode}"
                    );

                    Logger.LogActivity($"SERVICE ERROR: {JsonSerializer.Serialize(error)}", "ERROR");
                    throw new HttpRequestException(error.ToString());
                }

                Logger.LogActivity($"GRC PATCH Request completed for: {endpoint}", "INFO");
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Exception: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                throw;
            } catch (Exception ex) {
                Logger.LogActivity($"Unhandled Exception during PATCH to {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task<GrcResponse<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest data) where TResponse : class {
            try {
                Logger.LogActivity($"GRC POST Request to: {endpoint}", "INFO");
            
                Logger.LogActivity($"REQUEST MAP: {JsonSerializer.Serialize(data)}");
                var jsonContent = JsonSerializer.Serialize(data, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
                var fullUrl = $"{GrcHttpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                Logger.LogActivity($"MIDDLEWARE URL: {fullUrl}");

                var response = await GrcHttpClient.PostAsync(endpoint, content);
                if(response == null) { 
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADGATEWAY,
                        "Bad Gateway or possible timeout",
                        "The middleware service did not respond or service timeout occurred"
                    );
            
                    Logger.LogActivity($"BAD GATEWAY: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<TResponse>(error);
                }

                Logger.LogActivity($"Response Status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode) {
                    Logger.LogActivity($"Middleware call failed with status: {(int)response.StatusCode}", "WARNING");
            
                    var error = new GrcResponseError(
                        (GrcStatusCodes)(int)response.StatusCode,
                        "Could not complete registration. An Error occurred",
                        $"HTTP Status Code: {response.StatusCode}"
                    );
            
                    Logger.LogActivity($"SERVICE ERROR: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<TResponse>(error);
                }
        
                //..read and deserialize response
                var responseData = await response.Content.ReadAsStringAsync();
                Logger.LogActivity($"GRC POST Response received from: {endpoint}", "INFO");
                Logger.LogActivity($"Response Data: {responseData}");
                try {
                    var options = new JsonSerializerOptions { 
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true
                    };
    
                    Logger.LogActivity("Starting deserialization...");
                    var result = JsonSerializer.Deserialize<GrcResponse<TResponse>>(responseData, options);
    
                    if (result == null) {
                        Logger.LogActivity("Deserialization returned null", "ERROR");
                        var error = new GrcResponseError(
                            GrcStatusCodes.SERVERERROR,
                            "System Data Error",
                            "Deserialization returned null"
                        );
                        return new GrcResponse<TResponse>(error);
                    }
    
                    Logger.LogActivity($"Deserialization successful. HasError: {result.HasError}");
                    return result;
                } catch (JsonException jex) {
                    Logger.LogActivity($"Deserialization Failed: {jex.Message}", "ERROR");
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Failed to deserialize response. An error has occurred"
                    );
                    return new GrcResponse<TResponse>(error);
                } catch (Exception ex) { 
                    Logger.LogActivity($"Unexpected deserialization error: {ex.Message}", "ERROR");
                    Logger.LogActivity($"Exception Type: {ex.GetType().Name}", "ERROR");
                    Logger.LogActivity($"StackTrace: {ex.StackTrace}", "ERROR");
    
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Unexpected deserialization error: {ex.Message}"
                    );
                    return new GrcResponse<TResponse>(error);
                }
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (TaskCanceledException timeoutEx) when (timeoutEx.InnerException is TimeoutException) {
                Logger.LogActivity("Request timeout", "ERROR");
                Logger.LogActivity(timeoutEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.TIMEOUT,
                    "Request timeout",
                    "The request took too long to complete"
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (JsonException jsonEx) {
                Logger.LogActivity($"JSON Deserialization Error: {jsonEx.Message}", "ERROR");
                Logger.LogActivity(jsonEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Ooops! Sorry, something went wrong",
                    "Data format error. Could not format data"
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (GRCException ex)  {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<TResponse>(error);
            }
        }

        public async Task PostAsync<TRequest>(string endpoint, TRequest data) {
            try {
                Logger.LogActivity($"GRC POST Request (no response) to: {endpoint}", "INFO");
                Logger.LogActivity($"REQUEST MAP: {JsonSerializer.Serialize(data)}");
                var jsonContent = JsonSerializer.Serialize(data, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
                var requestUrl = $"{GrcHttpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                Logger.LogActivity($"REQUEST URL: {requestUrl}");

                var response = await GrcHttpClient.PostAsync(endpoint, content);
                if (response == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADGATEWAY,
                        "Bad Gateway or possible timeout",
                        "The middleware service did not respond or service timeout occurred"
                    );
                    Logger.LogActivity($"BAD GATEWAY: {JsonSerializer.Serialize(error)}", "ERROR");
                    throw new HttpRequestException(error.ToString());
                }
                
                Logger.LogActivity($"Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode) {
                    Logger.LogActivity($"Middleware call failed with status: {(int)response.StatusCode}", "WARNING");

                    var error = new GrcResponseError(
                        (GrcStatusCodes)(int)response.StatusCode,
                        "Could not complete PATCH request. An error occurred",
                        $"HTTP Status Code: {response.StatusCode}"
                    );

                    Logger.LogActivity($"SERVICE ERROR: {JsonSerializer.Serialize(error)}", "ERROR");
                    throw new HttpRequestException(error.ToString());
                }
            
                Logger.LogActivity($"GRC POST Request completed for: {endpoint}", "INFO");
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Exception: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                throw;
            } catch (Exception ex) {
                Logger.LogActivity($"Unhandled Exception during PATCH to {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task<GrcResponse<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest data) where TResponse : class {
            try {
                Logger.LogActivity($"GRC PUT Request to: {endpoint}", "INFO");
                
                //..formulate URL
                var fullUrl = $"{GrcHttpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                Logger.LogActivity($"MIDDLEWARE URL: {fullUrl}");

                var jsonContent = JsonSerializer.Serialize(data, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
                var response = await GrcHttpClient.PutAsync(endpoint, content);
                if(response == null) { 
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADGATEWAY,
                        "Bad Gateway or possible timeout",
                        "The middleware service did not respond or service timeout occurred"
                    );

                    Logger.LogActivity($"BAD GATEWAY: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<TResponse>(error);
                }

                //..we received a response
                Logger.LogActivity($"Response Status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode) {
                    Logger.LogActivity($"Middleware call failed with status: {(int)response.StatusCode}", "WARNING");

                    var error = new GrcResponseError(
                        (GrcStatusCodes)(int)response.StatusCode,
                        "Could not complete request. An Error occurred",
                        $"HTTP Status Code: {response.StatusCode}"
                    );

                    Logger.LogActivity($"SERVICE ERROR: {JsonSerializer.Serialize(error)}");

                    return new GrcResponse<TResponse>(error);
                }

                //..read and deserialize response
                var responseData = await response.Content.ReadAsStringAsync();
                Logger.LogActivity($"GRC PUT Response received from: {endpoint}", "INFO");
                Logger.LogActivity($"GRC Midleware data : {responseData}");
            
                try {

                    Logger.LogActivity("Starting deserialization...");
                    var result = JsonSerializer.Deserialize<GrcResponse<TResponse>>(responseData, JsonOptions);
    
                    if (result == null) {
                        Logger.LogActivity("Deserialization returned null", "ERROR");
                        var error = new GrcResponseError(
                            GrcStatusCodes.SERVERERROR,
                            "System Data Error",
                            "Deserialization returned null"
                        );
                        return new GrcResponse<TResponse>(error);
                    }
    
                    Logger.LogActivity($"Deserialization successful. HasError: {result.HasError}");
                    Logger.LogActivity($"SERVICE RESULT: {JsonSerializer.Serialize(result, JsonOptions)}");
                    return result;
                } catch (JsonException jex) {
                    Logger.LogActivity($"Deserialization Failed: {jex.Message}", "ERROR");
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Failed to deserialize response. An error has occurred"
                    );
                    return new GrcResponse<TResponse>(error);
                } catch (Exception ex) { 
                    Logger.LogActivity($"Unexpected deserialization error: {ex.Message}", "ERROR");
                    Logger.LogActivity($"Exception Type: {ex.GetType().Name}", "ERROR");
                    Logger.LogActivity($"StackTrace: {ex.StackTrace}", "ERROR");
    
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Unexpected deserialization error: {ex.Message}"
                    );
                    return new GrcResponse<TResponse>(error);
                }
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (TaskCanceledException timeoutEx) when (timeoutEx.InnerException is TimeoutException) {
                Logger.LogActivity("Request timeout", "ERROR");
                Logger.LogActivity(timeoutEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.TIMEOUT,
                    "Request timeout",
                    "The request took too long to complete"
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (JsonException jsonEx) {
                Logger.LogActivity($"JSON Deserialization Error: {jsonEx.Message}", "ERROR");
                Logger.LogActivity(jsonEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Ooops! Sorry, something went wrong",
                    "Data format error. Could not format data"
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (GRCException ex)  {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<TResponse>(error);
            }
        }

        public async Task PutAsync<TRequest>(string endpoint, TRequest data) {
            try {
                Logger.LogActivity($"GRC PUT Request (no response) to: {endpoint}", "INFO");
                Logger.LogActivity($"REQUEST MAP: {JsonSerializer.Serialize(data)}");
            
                var jsonContent = JsonSerializer.Serialize(data, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
                var requestUrl = $"{GrcHttpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                Logger.LogActivity($"REQUEST URL: {requestUrl}");

                var response = await GrcHttpClient.PutAsync(endpoint, content);
                if (response == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADGATEWAY,
                        "Bad Gateway or possible timeout",
                        "The middleware service did not respond or service timeout occurred"
                    );
                    Logger.LogActivity($"BAD GATEWAY: {JsonSerializer.Serialize(error)}", "ERROR");
                    throw new HttpRequestException(error.ToString());
                }

                Logger.LogActivity($"Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode) {
                    Logger.LogActivity($"Middleware call failed with status: {(int)response.StatusCode}", "WARNING");

                    var error = new GrcResponseError(
                        (GrcStatusCodes)(int)response.StatusCode,
                        "Could not complete PATCH request. An error occurred",
                        $"HTTP Status Code: {response.StatusCode}"
                    );

                    Logger.LogActivity($"SERVICE ERROR: {JsonSerializer.Serialize(error)}", "ERROR");
                    throw new HttpRequestException(error.ToString());
                }
            
                Logger.LogActivity($"GRC PUT Request completed for: {endpoint}", "INFO");
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Exception: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                throw;
            } catch (Exception ex) {
                Logger.LogActivity($"Unhandled Exception during PATCH to {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }
        
        public async Task<GrcResponse<TResponse>> DeleteAsync<TResponse>(string endpoint) where TResponse : class {
            try {
                Logger.LogActivity($"DELETE Request to: {endpoint}", "INFO");
                
                //..formulate URL
                var fullUrl = $"{GrcHttpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                Logger.LogActivity($"MIDDLEWARE URL: {fullUrl}");

                //..send delete request
                var response = await GrcHttpClient.DeleteAsync(endpoint);
                if(response == null) { 
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADGATEWAY,
                        "Bad Gateway or possible timeout",
                        "The middleware service did not respond or service timeout occurred"
                    );

                    Logger.LogActivity($"BAD GATEWAY: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<TResponse>(error);
                }

                //..we received a response
                Logger.LogActivity($"Response Status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode) {
                    Logger.LogActivity($"Middleware call failed with status: {(int)response.StatusCode}", "WARNING");

                    var error = new GrcResponseError(
                        (GrcStatusCodes)(int)response.StatusCode,
                        "Could not complete request. An Error occurred",
                        $"HTTP Status Code: {response.StatusCode}"
                    );

                    Logger.LogActivity($"SERVICE ERROR: {JsonSerializer.Serialize(error)}");

                    return new GrcResponse<TResponse>(error);
                }
        
                //..read and deserialize response
                var responseData = await response.Content.ReadAsStringAsync();
                Logger.LogActivity($"DELETE Response received from: {endpoint}", "INFO");
                Logger.LogActivity($"GRC Midleware data : {responseData}");
                try {

                    Logger.LogActivity("Starting deserialization...");
                    var result = JsonSerializer.Deserialize<GrcResponse<TResponse>>(responseData, JsonOptions);
    
                    if (result == null) {
                        Logger.LogActivity("Deserialization returned null", "ERROR");
                        var error = new GrcResponseError(
                            GrcStatusCodes.SERVERERROR,
                            "System Data Error",
                            "Deserialization returned null"
                        );
                        return new GrcResponse<TResponse>(error);
                    }
    
                    Logger.LogActivity($"Deserialization successful. HasError: {result.HasError}");
                    Logger.LogActivity($"SERVICE RESULT: {JsonSerializer.Serialize(result, JsonOptions)}");
                    return result;
                } catch (JsonException jex) {
                    Logger.LogActivity($"Deserialization Failed: {jex.Message}", "ERROR");
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Failed to deserialize response. An error has occurred"
                    );
                    return new GrcResponse<TResponse>(error);
                } catch (Exception ex) { 
                    Logger.LogActivity($"Unexpected deserialization error: {ex.Message}", "ERROR");
                    Logger.LogActivity($"Exception Type: {ex.GetType().Name}", "ERROR");
                    Logger.LogActivity($"StackTrace: {ex.StackTrace}", "ERROR");
    
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Unexpected deserialization error: {ex.Message}"
                    );
                    return new GrcResponse<TResponse>(error);
                }
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (TaskCanceledException timeoutEx) when (timeoutEx.InnerException is TimeoutException) {
                Logger.LogActivity("Request timeout", "ERROR");
                Logger.LogActivity(timeoutEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.TIMEOUT,
                    "Request timeout",
                    "The request took too long to complete"
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (JsonException jsonEx) {
                Logger.LogActivity($"JSON Deserialization Error: {jsonEx.Message}", "ERROR");
                Logger.LogActivity(jsonEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Ooops! Sorry, something went wrong",
                    "Data format error. Could not format data"
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (GRCException ex)  {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<TResponse>(error);
            }
        }

        public async Task DeleteAllAsync(string endpoint) {
            try {
                Logger.LogActivity($"GRC DELETE Request (no response) to: {endpoint}", "INFO");
            
                var requestUrl = $"{GrcHttpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                Logger.LogActivity($"REQUEST URL: {requestUrl}");

                var response = await GrcHttpClient.DeleteAsync(endpoint);
                if (response == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADGATEWAY,
                        "Bad Gateway or possible timeout",
                        "The middleware service did not respond or service timeout occurred"
                    );
                    Logger.LogActivity($"BAD GATEWAY: {JsonSerializer.Serialize(error)}", "ERROR");
                    throw new HttpRequestException(error.ToString());
                }

                Logger.LogActivity($"Response Status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode) {
                    Logger.LogActivity($"Middleware call failed with status: {(int)response.StatusCode}", "WARNING");

                    var error = new GrcResponseError(
                        (GrcStatusCodes)(int)response.StatusCode,
                        "Could not complete PATCH request. An error occurred",
                        $"HTTP Status Code: {response.StatusCode}"
                    );

                    Logger.LogActivity($"SERVICE ERROR: {JsonSerializer.Serialize(error)}", "ERROR");
                    throw new HttpRequestException(error.ToString());
                }
            
                Logger.LogActivity($"GRC DELETE Request completed for: {endpoint}", "INFO");
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Exception: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                throw;
            } catch (Exception ex) {
                Logger.LogActivity($"Unhandled Exception during DELETEALL to {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }

        public async Task<GrcResponse<TResponse>> SendAsync<TResponse>(HttpMethod method, string endpoint, object requestBody = null) where TResponse: class {
            try  {
                Logger.LogActivity($"{method.Method} Request to: {endpoint}", "INFO");
            
                //..formulate URL
                var fullUrl = $"{GrcHttpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                Logger.LogActivity($"MIDDLEWARE URL: {fullUrl}");

                //..send request
                var request = new HttpRequestMessage(method, endpoint);
                if (requestBody != null) {
                    var jsonContent = JsonSerializer.Serialize(requestBody, JsonOptions);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    Logger.LogActivity($"REQUEST MAP: {JsonSerializer.Serialize(requestBody)}");
                }
            
                 //..send request
                var response = await GrcHttpClient.SendAsync(request);
                if(response == null) { 
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADGATEWAY,
                        "Bad Gateway or possible timeout",
                        "The middleware service did not respond or service timeout occurred"
                    );

                    Logger.LogActivity($"BAD GATEWAY: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<TResponse>(error);
                }

                //..we received a response
                Logger.LogActivity($"Response Status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode) {
                    Logger.LogActivity($"Middleware call failed with status: {(int)response.StatusCode}", "WARNING");

                    var error = new GrcResponseError(
                        (GrcStatusCodes)(int)response.StatusCode,
                        "Could not complete request. An Error occurred",
                        $"HTTP Status Code: {response.StatusCode}"
                    );

                    Logger.LogActivity($"SERVICE ERROR: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<TResponse>(error);
                }
        
                //..read and deserialize response
                var responseData = await response.Content.ReadAsStringAsync();
                Logger.LogActivity($"{method.Method} Response received from: {endpoint}", "INFO");
                Logger.LogActivity($"GRC Midleware data : {responseData}");
            
                try {

                    Logger.LogActivity("Starting deserialization...");
                    var result = JsonSerializer.Deserialize<GrcResponse<TResponse>>(responseData, JsonOptions);
    
                    if (result == null) {
                        Logger.LogActivity("Deserialization returned null", "ERROR");
                        var error = new GrcResponseError(
                            GrcStatusCodes.SERVERERROR,
                            "System Data Error",
                            "Deserialization returned null"
                        );
                        return new GrcResponse<TResponse>(error);
                    }
    
                    Logger.LogActivity($"Deserialization successful. HasError: {result.HasError}");
                    Logger.LogActivity($"SERVICE RESULT: {JsonSerializer.Serialize(result, JsonOptions)}");
                    return result;
                } catch (JsonException jex) {
                    Logger.LogActivity($"Deserialization Failed: {jex.Message}", "ERROR");
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Failed to deserialize response. An error has occurred"
                    );
                    return new GrcResponse<TResponse>(error);
                } catch (Exception ex) { 
                    Logger.LogActivity($"Unexpected deserialization error: {ex.Message}", "ERROR");
                    Logger.LogActivity($"Exception Type: {ex.GetType().Name}", "ERROR");
                    Logger.LogActivity($"StackTrace: {ex.StackTrace}", "ERROR");
    
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Unexpected deserialization error: {ex.Message}"
                    );
                    return new GrcResponse<TResponse>(error);
                }
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (TaskCanceledException timeoutEx) when (timeoutEx.InnerException is TimeoutException) {
                Logger.LogActivity("Request timeout", "ERROR");
                Logger.LogActivity(timeoutEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.TIMEOUT,
                    "Request timeout",
                    "The request took too long to complete"
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (JsonException jsonEx) {
                Logger.LogActivity($"JSON Deserialization Error: {jsonEx.Message}", "ERROR");
                Logger.LogActivity(jsonEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Ooops! Sorry, something went wrong",
                    "Data format error. Could not format data"
                );
                return new GrcResponse<TResponse>(error);
        
            } catch (GRCException ex)  {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<TResponse>(error);
            }
        }

        public async Task SendAsync(HttpMethod method, string endpoint, object requestBody = null) {
            try  {
                Logger.LogActivity($"{method.Method} Request (no response) to: {endpoint}", "INFO");
                
                var request = new HttpRequestMessage(method, endpoint);
                if (requestBody != null) {
                    var jsonContent = JsonSerializer.Serialize(requestBody, JsonOptions);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    Logger.LogActivity($"REQUEST MAP: {JsonSerializer.Serialize(requestBody)}");
                }

                var requestUrl = $"{GrcHttpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                Logger.LogActivity($"REQUEST URL: {requestUrl}");
            
                var response = await GrcHttpClient.SendAsync(request);
                if (response == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADGATEWAY,
                        "Bad Gateway or possible timeout",
                        "The middleware service did not respond or service timeout occurred"
                    );
                    Logger.LogActivity($"BAD GATEWAY: {JsonSerializer.Serialize(error)}", "ERROR");
                    throw new HttpRequestException(error.ToString());
                }

                Logger.LogActivity($"Response Status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode) {
                    Logger.LogActivity($"Middleware call failed with status: {(int)response.StatusCode}", "WARNING");

                    var error = new GrcResponseError(
                        (GrcStatusCodes)(int)response.StatusCode,
                        "Could not complete PATCH request. An error occurred",
                        $"HTTP Status Code: {response.StatusCode}"
                    );

                    Logger.LogActivity($"SERVICE ERROR: {JsonSerializer.Serialize(error)}", "ERROR");
                    throw new HttpRequestException(error.ToString());
                }
            
                Logger.LogActivity($"{method.Method} Request completed for: {endpoint}", "INFO");
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Exception: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                throw;
            } catch (Exception ex)  {
                Logger.LogActivity($"{method.Method} Error for endpoint {endpoint}: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                throw;
            }
        }
        
    }

}
