using Grc.ui.App.Dtos;
using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IOperationsDashboardFactory {
        Task<OperationsDashboardModel> PrepareDefaultOperationsModelAsync(UserModel model);
        Task<TotalExtensionModel> PrepareDefaultTotalExtensionsModelAsync(UserModel model);
        Task<OperationsDashboardModel> PrepareOperationsDashboardModelAsync(UserModel model);
        Task<OperationsDashboardModel> PrepareUnitStatisticsModelAsync(UserModel model, string unit);
        Task<CategoryExtensionModel> PrepareCategoryExtensionsModelAsync(UserModel model, string category);
        Task<TotalExtensionModel> PrepareExtensionCategoryErrorModelAsync(UserModel model);
        Task<CategoryExtensionModel> PrepareDefaultExtensionCategoryErrorModelAsync(UserModel model);
        Task<OperationsDashboardModel> PrepareErrorOperationsDashboardModelAsync(UserModel model);
        Task<UnitExtensionModel> PrepareUnitExtensionsModelAsync(UserModel model, string category);
        Task<UnitExtensionModel> PrepareDefaultExtensionUnitErrorModelAsync(UserModel model);
    }
}
   