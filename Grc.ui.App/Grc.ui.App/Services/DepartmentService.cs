using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Text.Json;

namespace Grc.ui.App.Services {

    public class DepartmentService : GrcBaseService, IDepartmentService {

        public DepartmentService(IApplicationLoggerFactory loggerFactory, 
                                    IHttpHandler httpHandler, 
                                    IEnvironmentProvider environment, 
                                    IEndpointTypeProvider endpointType, 
                                    IMapper mapper,
                                    IWebHelper webHelper,
                                    SessionManager sessionManager,
                                    IGrcErrorFactory errorFactory,
                                    IErrorService errorService) 
                            : base(loggerFactory, httpHandler, environment, 
                                  endpointType, mapper,webHelper,
                                  sessionManager,errorFactory,errorService) {
        }

        public async Task<GrcResponse<PagedResponse<DepartmentModel>>> GetDepartmentsAsync(TableListRequest request) {
            Logger.LogActivity($"Get a list of all departments", "INFO");

            try{
               var endpoint = $"{EndpointProvider.Departments.AllDepartments}";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<DepartmentModel>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving list of departments: {ex.Message}", "Error");
                Logger.LogActivity($"Unexpected Update error: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-SERVICE" , ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Error retrieving list of departments",
                    ex.Message
                );

                return new GrcResponse<PagedResponse<DepartmentModel>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> InsertDepartmentAsync(DepartmentModel model, long userId, string ipAddress = null) {
            try {
                //..build request model
                var request = new DepartmentRequest() {
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "Insert new department",
                    DepartmentCode = model.DepartmentCode,
                    DepartmentName = model.DepartmentName,
                    BranchId = model.BranchId,
                    IsDeleted = model.IsDeleted
                };

                //..map request
                Logger.LogActivity($"DEPARTMENT REQUEST : {JsonSerializer.Serialize(request)}");
                
                //..build endpoint
                var endpoint = $"{EndpointProvider.Departments.InsertDepartment}";
                Logger.LogActivity($"Endpoint: {endpoint}");
        
                return await HttpHandler.PostAsync<DepartmentRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message,"DEPARTMENT-SERVICE" , httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);
        
            } catch (GRCException ex)  {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-SERVICE" , ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateDepartmentAsync(DepartmentModel model, long userId, string ipAddress = null) {
            try {

                //..build request model
                var request = new DepartmentRequest() {
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "Update department",
                    DepartmentCode = model.DepartmentCode,
                    DepartmentName = model.DepartmentName,
                    BranchId = model.BranchId,
                    IsDeleted = model.IsDeleted
                };

                var endpoint = EndpointProvider.Departments.UpdateDepartment;
                var response = await HttpHandler.PostAsync<DepartmentRequest, StatusResponse>(endpoint, request);
                if(response.HasError) { 
                    Logger.LogActivity($"Failed to update department on server. {response.Error.Message}");
                } else {
                    Logger.LogActivity("Department updated successfully.");
                }
                
                return await HttpHandler.PostAsync<DepartmentRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"Http Exception: {httpEx.Message}", "ERROR");
                await ProcessErrorAsync(httpEx.Message,"DEPARTMENT-SERVICE" , httpEx.StackTrace);
                throw new GRCException("Failed to update department. Error.", httpEx);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Update error: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-SERVICE" , ex.StackTrace);
                throw new GRCException("An unexpected error occurred during update of department.", ex);
            }
        }
        
        public async Task<GrcResponse<ServiceResponse>> DeleteDepartmentAsync(long id, long userId, string ipAddress = null) {
            try {
                var request = new GrcDeleteRequst(){ 
                    RecordId = id,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "Delete Department"
                };

                var endpoint = EndpointProvider.Departments.DeleteDepartment;
                var response = await HttpHandler.PostAsync<GrcDeleteRequst, StatusResponse>(endpoint, request);
                if(response.HasError) { 
                    Logger.LogActivity($"Failed to delete department on server. {response.Error.Message}");
                } else {
                    Logger.LogActivity("Department deleted successfully.");
                }
                
                return await HttpHandler.PostAsync<GrcDeleteRequst, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"Http Exception: {httpEx.Message}", "ERROR");
                await ProcessErrorAsync(httpEx.Message,"DEPARTMENT-SERVICE" , httpEx.StackTrace);
                throw new GRCException("Failed to update department. Error.", httpEx);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Update error: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-SERVICE" , ex.StackTrace);
                throw new GRCException("An unexpected error occurred during update of department.", ex);
            }
        }

    }
}
