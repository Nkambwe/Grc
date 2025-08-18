using Grc.Middleware.Api.Http.Requests;

namespace Grc.Middleware.Api.Services {
    public interface IPinnedItemService {
        Task<IList<PinnedItemResponse>> GetUserPinnedItemsAsync(long recordId);
    }
}
