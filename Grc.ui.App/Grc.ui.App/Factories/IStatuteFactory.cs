using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IStatuteFactory {
        Task<StatutoryViewModel> PrepareStatuteViewModelAsync(UserModel currentUser);
    }
}
