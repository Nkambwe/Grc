using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Grc.ui.App.Services {

    public class InstallService : GrcBaseService, IInstallService {

        public InstallService(IApplicationLoggerFactory loggerFactory, 
                              IHttpClientFactory httpClientFactory,
                              IEnvironmentProvider environment, 
                              IEndpointTypeProvider endpointType,
                              IMapper mapper)
            : base(loggerFactory, httpClientFactory, environment,endpointType, mapper) {
            Logger.Channel = $"COMPANY-{DateTime.Now:yyyyMMddHHmmss}";
        }

        public async Task<GrcResponse<ServiceResponse>> RegisterCompanyAsync(CompanyRegistrationModel model, string ipAddress) {
            //..validate input
            if(model == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Request record cannot be empty",
                    "The company registration model cannot be null"
                );
        
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..map request
                Logger.LogActivity($"REQUEST MODEL: {JsonSerializer.Serialize(model)}");
                var request = Mapper.Map<CompanyRegiatrationRequest>(model);
                request.IPAddress = ipAddress;

                Logger.LogActivity($"REQUEST MAP: {JsonSerializer.Serialize(request)}");
                var jsonContent = JsonSerializer.Serialize(request, JsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Registration.Register}";
                Logger.LogActivity($"Endpoint: {endpoint}");
        
                var fullUrl = $"{GrcHttpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
                Logger.LogActivity($"MIDDLEWARE URL: {fullUrl}");

                //..send request
                var svrcResponse = await GrcHttpClient.PostAsync(endpoint, content);
                if(svrcResponse == null) { 
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADGATEWAY,
                        "Bad Gateway or possible timeout",
                        "The middleware service did not respond or service timeout occurred"
                    );
            
                    Logger.LogActivity($"BAD GATEWAY: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                Logger.LogActivity($"Response Status: {svrcResponse.StatusCode}");
                if (!svrcResponse.IsSuccessStatusCode) {
                    Logger.LogActivity($"Middleware call failed with status: {(int)svrcResponse.StatusCode}", "WARNING");
            
                    var error = new GrcResponseError(
                        (GrcStatusCodes)(int)svrcResponse.StatusCode,
                        "Could not complete registration. An Error occurred",
                        $"HTTP Status Code: {svrcResponse.StatusCode}"
                    );
            
                    Logger.LogActivity($"SERVICE ERROR: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..read and deserialize response
                var responseData = await svrcResponse.Content.ReadAsStringAsync();
                Logger.LogActivity($"Response Data: {responseData}");
                try {
                    var options = new JsonSerializerOptions { 
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true
                    };
    
                    Logger.LogActivity("Starting deserialization...");
                    var result = JsonSerializer.Deserialize<GrcResponse<ServiceResponse>>(responseData, options);
    
                    if (result == null) {
                        Logger.LogActivity("Deserialization returned null", "ERROR");
                        var error = new GrcResponseError(
                            GrcStatusCodes.SERVERERROR,
                            "System Data Error",
                            "Deserialization returned null"
                        );
                        return new GrcResponse<ServiceResponse>(error);
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
                    return new GrcResponse<ServiceResponse>(error);
                } catch (Exception ex) { 
                    Logger.LogActivity($"Unexpected deserialization error: {ex.Message}", "ERROR");
                    Logger.LogActivity($"Exception Type: {ex.GetType().Name}", "ERROR");
                    Logger.LogActivity($"StackTrace: {ex.StackTrace}", "ERROR");
    
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "System Data Error",
                        $"Unexpected deserialization error: {ex.Message}"
                    );
                    return new GrcResponse<ServiceResponse>(error);
                }

            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);
        
            } catch (TaskCanceledException timeoutEx) when (timeoutEx.InnerException is TimeoutException) {
                Logger.LogActivity("Request timeout", "ERROR");
                Logger.LogActivity(timeoutEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.TIMEOUT,
                    "Request timeout",
                    "The request took too long to complete"
                );
                return new GrcResponse<ServiceResponse>(error);
        
            } catch (JsonException jsonEx) {
                Logger.LogActivity($"JSON Deserialization Error: {jsonEx.Message}", "ERROR");
                Logger.LogActivity(jsonEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Ooops! Sorry, something went wrong",
                    "Data format error. Could not format data"
                );
                return new GrcResponse<ServiceResponse>(error);
        
            } catch (GRCException ex)  {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }
    }

}
