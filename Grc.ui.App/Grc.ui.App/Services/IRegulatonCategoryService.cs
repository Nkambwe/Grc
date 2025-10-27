using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IRegulatonCategoryService : IGrcBaseService {
        Task<GrcResponse<RegulatoryCategoryResponse>> GetCategoryAsync(GrcIdRequest deleteRequest);
        Task<GrcResponse<PagedResponse<RegulatoryCategoryResponse>>> GetAllRegulatoryCategories(TableListRequest request);
        Task<GrcResponse<List<RegulatoryCategoryResponse>>> GetRegulatoryCategories(GrcRequest request);
        Task<GrcResponse<RegulatoryCategoryResponse>> CreateCategoryAsync(RegulatoryCategoryRequest request);
        Task<GrcResponse<RegulatoryCategoryResponse>> UpdateCategoryAsync(RegulatoryCategoryRequest request);
        Task<GrcResponse<ServiceResponse>> DeleteCategoryAsync(GrcIdRequest deleteRequest);
        
    }
}
