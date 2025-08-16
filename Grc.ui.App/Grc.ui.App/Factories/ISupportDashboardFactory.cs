using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface ISupportDashboardFactory{ 
        Task<AdminDashboardModel>  PrepareAdminDashboardModelAsync(UserModel model);
    }
}
