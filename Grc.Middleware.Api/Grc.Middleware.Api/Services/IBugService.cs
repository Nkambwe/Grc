using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;

namespace Grc.Middleware.Api.Services {
    public interface IBugService : IBaseService { 
        Task<PagedResult<SystemError>> GetBugsAsync(BugListRequest request);
    }
}
