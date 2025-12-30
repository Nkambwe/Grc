using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {
    public interface IComplianceControlService {
        Task<GrcResponse<PagedResponse<GrcControlCategoryResponse>>> GetControlCategoriesAsync(TableListRequest request);
        Task<GrcResponse<ServiceResponse>> CreateControlCategoryAsync(ControlCategoryViewModel model, long userId, string ipAddress);
        Task<GrcResponse<GrcControlCategoryResponse>> GetCategoryAsync(GrcIdRequest request);
        Task<GrcResponse<GrcControlItemResponse>> GetItemAsync(GrcIdRequest request);
        Task<GrcResponse<ServiceResponse>> DeleteItemAsync(GrcIdRequest request);
        Task<GrcResponse<ServiceResponse>> CreateItemAsync(ItemViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateCategoryAsync(ControlCategoryViewModel request, long userId, string ipAddress);
        Task<GrcResponse<ServiceResponse>> UpdateItemAsync(ItemViewModel request, long userId, string ipAddress);
    }
}
