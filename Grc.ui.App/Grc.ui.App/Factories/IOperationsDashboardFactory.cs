using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IOperationsDashboardFactory
    {
        Task<OperationsDashboardModel> PrepareDefaultOperationsModelAsync(UserModel model);
        Task<OperationsDashboardModel> PrepareOperationsDashboardModelAsync(UserModel model);
    }
}
   