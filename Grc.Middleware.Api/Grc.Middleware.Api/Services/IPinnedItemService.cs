using Grc.Middleware.Api.Http.Requests;

namespace Grc.Middleware.Api.Services {
    public interface IPinnedItemService : IBaseService {
        Task<IList<PinnedItemResponse>> GetUserPinnedItemsAsync(long recordId);
    }
}
