using AutoMapper;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;

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

        public Task<UserModel> GetUserByEmailAsync(string email) {
            throw new NotImplementedException();
        }

        public Task<UserModel> GetUserByIdAsync(long userId) {
            throw new NotImplementedException();
        }

        public Task<UserModel> GetUserByUsernameAsync(string username) {
            throw new NotImplementedException();
        }
    }

}
