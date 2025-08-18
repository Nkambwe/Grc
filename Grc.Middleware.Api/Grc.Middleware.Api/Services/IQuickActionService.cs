
using Grc.Middleware.Api.Http.Requests;

namespace Grc.Middleware.Api.Services {
    public interface IQuickActionService {
        Task<IList<QuickActionResponse>> GetUserQuickActionAsync(long recordId);
    }
}
