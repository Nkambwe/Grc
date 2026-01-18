using Grc.ui.App.Dtos;
using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {

    public interface ISupportDashboardFactory{ 
        Task<AdminDashboardModel>  PrepareDefaultModelAsync(UserModel model);
        Task<AdminDashboardModel>  PrepareAdminDashboardModelAsync(UserModel model);
        Task<RoleGroupListModel> PrepareRoleGroupListModelAsync(UserModel model);
        Task<OperationProcessViewModel> PrepareProcessViewModelAsync(UserModel model);
        Task<UserSupportViewModel> PrepareUserSupportModelAsync(UserModel currentUser) ;
    }

}
   