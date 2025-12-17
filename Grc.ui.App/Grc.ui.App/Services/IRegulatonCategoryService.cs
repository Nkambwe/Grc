using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {
    public interface IRegulatonCategoryService : IGrcBaseService {
        Task<GrcResponse<GrcRegulatoryCategoryResponse>> GetCategoryAsync(GrcIdRequest request);
        Task<GrcResponse<PagedResponse<GrcRegulatoryCategoryResponse>>> GetPagedCategoriesAsync(TableListRequest request);
        Task<GrcResponse<List<GrcRegulatoryCategoryResponse>>> GetRegulatoryCategories(GrcRequest request);
        Task<GrcResponse<ServiceResponse>> CreateCategoryAsync(RegulatoryCategoryRequest request);
        Task<GrcResponse<ServiceResponse>> UpdateCategoryAsync(RegulatoryCategoryRequest request);
        Task<GrcResponse<ServiceResponse>> DeleteCategoryAsync(GrcIdRequest request);
        
    }
}
