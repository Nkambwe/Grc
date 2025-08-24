
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {

    public interface IBranchService {
        Task<GrcResponse<WorkspaceResponse>> GetWorkspaceAsync(long userId, long requestingUserId, string ipAddress);
    }
}
