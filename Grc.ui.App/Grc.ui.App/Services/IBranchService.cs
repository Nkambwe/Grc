
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {

    public interface IBranchService {
        Task<GrcResponse<PagedResponse<BranchResponse>>> GetAllBranchesAsync(TableListRequest request);
        Task<GrcResponse<List<BranchResponse>>> GetBranchesAsync(GrcRequest request);
        Task<GrcResponse<WorkspaceResponse>> GetWorkspaceAsync(long userId, long requestingUserId, string ipAddress);
    }
}
