using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;

namespace Grc.Middleware.Api.Services {
    public interface IBugService : IBaseService { 
        Task<PagedResult<BugItemResponse>> GetBugsAsync(BugListRequest request);
    }
}
