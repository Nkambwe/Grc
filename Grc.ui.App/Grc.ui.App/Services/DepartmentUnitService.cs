using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Services {

    public class DepartmentUnitService : GrcBaseService, IDepartmentUnitService {

        public DepartmentUnitService(IApplicationLoggerFactory loggerFactory, 
                                    IHttpHandler httpHandler, 
                                    IEnvironmentProvider environment, 
                                    IEndpointTypeProvider endpointType, 
                                    IMapper mapper) 
                                    : base(loggerFactory, httpHandler, environment, endpointType, mapper) {
        }

        public async Task<GrcResponse<PagedResponse<DepartmentUnitModel>>> GetDepartmentUnitsAsync(TableListRequest request) {
            Logger.LogActivity($"Get a list of all department units", "INFO");

            try {
               var endpoint = $"{EndpointProvider.Departments.AllUnits}";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<DepartmentUnitModel>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving list of department units: {ex.Message}", "Error");
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Error retrieving list of department units",
                    ex.Message
                );

                return new GrcResponse<PagedResponse<DepartmentUnitModel>>(error);
            }

        }

        public Task<GrcResponse<ServiceResponse>> InsertDepartmentUnitAsync(DepartmentUnitModel model, long userId, string ipAddress = null) {
            throw new NotImplementedException();
        }

        public Task<GrcResponse<ServiceResponse>> UpdateDepartmentUnitAsync(DepartmentUnitModel model, long userId, string ipAddress = null) {
            throw new NotImplementedException();
        }
        
        public Task<GrcResponse<ServiceResponse>> DeleteDepartmentUnitAsync(long id, long userId, string ipAddress = null) {
            throw new NotImplementedException();
        }

    }
}
