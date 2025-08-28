
using Grc.Middleware.Api.Http.Requests;

namespace Grc.Middleware.Api.Services {
    public interface IQuickActionService : IBaseService {
        Task<IList<QuickActionResponse>> GetUserQuickActionAsync(long recordId);
    }
}
