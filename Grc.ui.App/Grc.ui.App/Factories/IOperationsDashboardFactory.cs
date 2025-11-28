using Grc.ui.App.Dtos;
using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IOperationsDashboardFactory {
        Task<OperationsDashboardModel> PrepareDefaultOperationsModelAsync(UserModel model);
        Task<TotalExtensionModel> PrepareDefaultTotalExtensionsModelAsync(UserModel model);
        Task<OperationsDashboardModel> PrepareOperationsDashboardModelAsync(UserModel model);
        Task<OperationsDashboardModel> PrepareUnitStatisticsModelAsync(UserModel model, string unit);
        Task<CategoryExtensionResponse> PrepareCategoryExtensionsModelAsync(UserModel model, string category);
        Task<TotalExtensionModel> PrepareExtensionCategoryErrorModelAsync(UserModel model);
        Task<CategoryExtensionResponse> PrepareDefaultExtensionCategoryErrorModelAsync(UserModel model);
        Task<OperationsDashboardModel> PrepareErrorOperationsDashboardModelAsync(UserModel model);
        Task<UnitExtensionCountResponse> PrepareUnitExtensionsModelAsync(UserModel model, string category);
        Task<UnitExtensionCountResponse> PrepareDefaultExtensionUnitErrorModelAsync(UserModel model);
    }
}
   