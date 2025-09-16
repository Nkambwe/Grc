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

    public class DepartmentUnitService : GrcBaseService, IDepartmentUnitService {

        public DepartmentUnitService(IApplicationLoggerFactory loggerFactory, 
                                    IHttpHandler httpHandler, 
                                    IEnvironmentProvider environment, 
                                    IEndpointTypeProvider endpointType, 
                                    IMapper mapper,
                                    IWebHelper webHelper,
                                    SessionManager sessionManager,
                                    IGrcErrorFactory errorFactory,
                                    IErrorService errorService) 
        : base(loggerFactory, httpHandler, environment, endpointType, mapper,webHelper,sessionManager,errorFactory,errorService) {
        }

        public async Task<GrcResponse<List<DepartmentUnitModel>>> GetUnitsAsync(GrcRequest request){ 
            Logger.LogActivity($"Get a list of units available", "INFO");

            try{
               var endpoint = $"{EndpointProvider.Departments.GetUnits}";
                return await HttpHandler.PostAsync<GrcRequest, List<DepartmentUnitModel>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving a list of units available: {ex.Message}", "Error");
                await ProcessErrorAsync(ex.Message,"DEPARTMENT-UNIT-SERVICE" , ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Error retrieving a list of available units",
                    ex.Message
                );

                return new GrcResponse<List<DepartmentUnitModel>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<DepartmentUnitModel>>> GetDepartmentUnitsAsync(TableListRequest request) {
            Logger.LogActivity($"Get a list of all department units", "INFO");

            try {
               var endpoint = $"{EndpointProvider.Departments.AllUnits}";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<DepartmentUnitModel>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving list of department units: {ex.Message}", "Error");
                await ProcessErrorAsync(ex.Message,"DEPARTMENT UNIT SERVICE" , ex.StackTrace);

                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Error retrieving list of department units",
                    ex.Message
                );

                return new GrcResponse<PagedResponse<DepartmentUnitModel>>(error);
            }

        }

        public async Task<GrcResponse<ServiceResponse>> InsertDepartmentUnitAsync(DepartmentUnitModel model, long userId, string ipAddress = null) {
            try {
                //..build request model
                var request = new DepartmentUnitRequest() {
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "Insert new department unit",
                    UnitCode = model.UnitCode,
                    UnitName = model.UnitName,
                    DepartmentId = model.DepartmentId,
                    IsDeleted = model.IsDeleted
                };

                //..map request
                Logger.LogActivity($"DEPARTMENT REQUEST : {JsonSerializer.Serialize(request)}");
                
                //..build endpoint
                var endpoint = $"{EndpointProvider.Departments.InsertUnit}";
                Logger.LogActivity($"Endpoint: {endpoint}");
        
                return await HttpHandler.PostAsync<DepartmentUnitRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message,"DEPARTMENTUNIT-SERVICE" , httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);
        
            } catch (GRCException ex)  {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message,"DEPARTMENTUNIT-SERVICE" , ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateDepartmentUnitAsync(DepartmentUnitModel model, long userId, string ipAddress = null) {
            try {

                //..build request model
                var request = new DepartmentUnitRequest() {
                    Id = model.Id,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "Update department unit",
                    UnitCode = model.UnitCode,
                    UnitName = model.UnitName,
                    DepartmentId = model.DepartmentId,
                    IsDeleted = model.IsDeleted
                };

                var endpoint = EndpointProvider.Departments.UpdateUnit;
                var response = await HttpHandler.PostAsync<DepartmentUnitRequest, StatusResponse>(endpoint, request);
                if(response.HasError) { 
                    Logger.LogActivity($"Failed to update department unit on server. {response.Error.Message}");
                } else {
                    Logger.LogActivity("Department unit updated successfully.");
                }
                
                return await HttpHandler.PostAsync<DepartmentUnitRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"Http Exception: {httpEx.Message}", "ERROR");
                await ProcessErrorAsync(httpEx.Message,"DEPARTMENTUNIT-SERVICE" , httpEx.StackTrace);
                throw new GRCException("Failed to update department unit. Error.", httpEx);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Update error: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"DEPARTMENTUNIT-SERVICE" , ex.StackTrace);
                throw new GRCException("An unexpected error occurred during update of department unit.", ex);
            }
        }
        
        public async Task<GrcResponse<ServiceResponse>> DeleteDepartmentUnitAsync(long id, long userId, string ipAddress = null) {
            try {
                var request = new GrcDeleteRequst(){ 
                    RecordId = id,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "Delete Department unit"
                };
                var endpoint = EndpointProvider.Departments.DeleteUnit;
                var response = await HttpHandler.PostAsync<GrcDeleteRequst, StatusResponse>(endpoint, request);
                if(response.HasError) { 
                    Logger.LogActivity($"Failed to delete department unit on server. {response.Error.Message}");
                } else {
                    Logger.LogActivity("Department unit deleted successfully.");
                }
                
                return await HttpHandler.PostAsync<GrcDeleteRequst, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"Http Exception: {httpEx.Message}", "ERROR");
                await ProcessErrorAsync(httpEx.Message,"DEPARTMENTUNIT-SERVICE" , httpEx.StackTrace);
                throw new GRCException("Failed to update department unit. Error.", httpEx);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Update error: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"DEPARTMENTUNIT-SERVICE" , ex.StackTrace);
                throw new GRCException("An unexpected error occurred during update of department unit.", ex);
            }
        }

    }
}
