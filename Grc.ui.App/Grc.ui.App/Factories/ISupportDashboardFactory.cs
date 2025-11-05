using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface ISupportDashboardFactory{ 
        Task<AdminDashboardModel>  PrepareDefaultModelAsync(UserModel model);
        Task<AdminDashboardModel>  PrepareAdminDashboardModelAsync(UserModel model);
        Task<RoleGroupListModel> PrepareRoleGroupListModelAsync(UserModel model);
    }
}
   