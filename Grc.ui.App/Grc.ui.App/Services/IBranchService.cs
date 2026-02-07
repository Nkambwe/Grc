using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IBranchService {
        Task<GrcResponse<BranchResponse>> GetBranchAsync(long recordId, long userId, string ipAddress);
        Task<GrcResponse<PagedResponse<BranchResponse>>> GetAllBranchesAsync(TableListRequest request);
        Task<GrcResponse<List<BranchResponse>>> GetBranchesAsync(GrcRequest request);
        Task<GrcResponse<ServiceResponse>> CreateBranchAsync(BranchModel model, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateBranchAsync(BranchModel model, long userId, string ipAddress);
        Task<GrcResponse<WorkspaceResponse>> GetWorkspaceAsync(long userId, long requestingUserId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> DeleteBranchAsync(GrcIdRequest request);
    }
}
