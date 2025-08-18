using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IPinnedService {
        Task<GrcResponse<IList<PinnedItem>>> GetPinnedItemAsync(long userId, string ipAddress); 
    }
}
