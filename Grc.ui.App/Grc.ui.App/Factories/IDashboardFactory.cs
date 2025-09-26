using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IDashboardFactory {
        Task<UserDashboardModel> PrepareUserDashboardModelAsync(UserModel model);
    }
}
