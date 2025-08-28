using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Http.Responses;

namespace Grc.Middleware.Api.Services {

    public interface ISystemErrorService : IBaseService {
        Task<BugCountResponse> GetErrorCountsAsync(long companyId);
        Task<bool> SaveErrorAsync(SystemError errorObj);
    }
}
