using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
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

        public async Task<GrcResponse<ServiceResponse>> RegisterCompanyAsync(CompanyRegistrationModel model) {
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
                var request = Mapper.Map<CompanyRegistrationModel>(model);

                //..TODO Map missing fields from automapper
                //{"CompanyName":"Pearl Bank Uganda","Alias":"PBU","RegistrationNumber":"C89001","SystemLanguage":"fr",
                //"SystemAdmin":{"Id":0,"FirstName":"Nkambwe","LastName":"Mark","MiddleName":"John","UserName":"mnkambwe","Password":"Password10!","ConfirmPassword":"Password10!","EmailAddress":"jo.jomac.mac801@gmail.com","PhoneNumber":"0773204651","PFNumber":"52603","SolId":null,"RoleId":0,"DepartmentId":0,"UnitCode":null,"IsActive":false,"IsVerified":false,"IsLogged":false,"CreatedOn":null,"CreatedBy":null,"ModifiedOn":null,"ModifiedBy":null}}

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

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ServiceResponse>(responseData, options);
        
                if (result == null) {
                    Logger.LogActivity("Deserialization Error! Unable to deserialize response object", "ERROR");
            
                    var error = new GrcResponseError(
                        GrcStatusCodes.SERVERERROR,
                        "Ooops! Sorry, something went wrong",
                        "Data format error. Could not format data"
                    );
            
                    Logger.LogActivity($"DESERIALIZATION ERROR: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                Logger.LogActivity($"SERVICE SUCCESS: {JsonSerializer.Serialize(result)}");
                return new GrcResponse<ServiceResponse>(result);

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
