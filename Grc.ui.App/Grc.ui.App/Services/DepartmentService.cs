using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Services {

    public class DepartmentService : GrcBaseService, IDepartmentService {

        public DepartmentService(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, 
            IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, IMapper mapper) 
            : base(loggerFactory, httpHandler, environment, endpointType, mapper) {
        }

        public async Task<GrcResponse<PagedResponse<DepartmentModel>>> GetDepartmentsAsync(TableListRequest request) {
            Logger.LogActivity($"Get a list of all departments", "INFO");

            try{
               var endpoint = $"{EndpointProvider.Departments.AllDepartments}";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<DepartmentModel>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving list of departments: {ex.Message}", "Error");
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Error retrieving list of departments",
                    ex.Message
                );

                return new GrcResponse<PagedResponse<DepartmentModel>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> InsertDepartmentAsync(DepartmentModel model, long userId, string ipAddress = null) {
            throw new NotImplementedException();
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateDepartmentAsync(DepartmentModel model, long userId, string ipAddress = null) {
            throw new NotImplementedException();
        }
        
        public Task<GrcResponse<ServiceResponse>> DeleteDepartmentAsync(long id, long userId, string ipAddress = null) {
            throw new NotImplementedException();
        }

    }
}
