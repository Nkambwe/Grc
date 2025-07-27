using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using System.Text.Json;

namespace Grc.ui.App.Http {

    public class SystemAccessService : GrcBaseService, ISystemAccessService {
        
        public SystemAccessService(IApplicationLoggerFactory loggerFactory, 
                                   IHttpHandler httpHandler,
                                   IEnvironmentProvider environment, 
                                   IEndpointTypeProvider endpointType,
                                   IMapper mapper)
            :base(loggerFactory, httpHandler, environment, endpointType, mapper){
            
        }

        public Task<int> CountActiveUsersAsync() {
            throw new NotImplementedException();
        }

        public Task<int> CountAllUsersAsync() {
            throw new NotImplementedException();
        }

        public Task<GrcResponse<UserModel>>  GetUserByEmailAsync(string email) {
            throw new NotImplementedException();
        }

        public Task<GrcResponse<UserModel>> GetUserByIdAsync(long userId) {
            throw new NotImplementedException();
        }

        public Task<GrcResponse<UserModel>> GetUserByUsernameAsync(string username) {
            throw new NotImplementedException();
        }

        public async Task<GrcResponse<UsernameValidationResponse>> ValidateUsernameAsync(UsernameValidationModel model) {

            try {
                //..map request
                var request = Mapper.Map<UsernameValidationRequest>(model);

                //..map endpoint
                var endpoint = $"{EndpointProvider.Sam.Users}/validate-username";

                //..post request
                return await HttpHandler.PostAsync<UsernameValidationRequest, UsernameValidationResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Authentication failed: {ex.Message}", "ERROR");
                var error = new GrcResponseError(
                    (int)GrcStatusCodes.SERVERERROR,
                    "Username validation failed, an error occurred",
                    ex.Message
                ); 
                Logger.LogActivity($"MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(error)}");
               return new GrcResponse<UsernameValidationResponse>(error);
            }
        }
    }

}
