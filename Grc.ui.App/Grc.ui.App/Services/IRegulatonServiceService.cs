using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services
{
    public interface IRegulatonServiceService : IGrcBaseService {
        Task<GrcResponse<PagedResponse<RegulatoryCategoryResponse>>> GetAllRegulatoryCategories(TableListRequest request);
        Task<GrcResponse<List<RegulatoryCategoryResponse>>> GetRegulatoryCategories(GrcRequest request);
    }
}
