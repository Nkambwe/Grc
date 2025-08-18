using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IQuickActionService {
        Task<GrcResponse<IList<QuickAction>>> GetQuickActionsync(long userId, string ipAddress); 
    }
}
