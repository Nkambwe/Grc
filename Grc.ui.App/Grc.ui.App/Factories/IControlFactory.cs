using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IControlFactory {
        Task<ControlViewModel> PrepareControlViewModelAsync(UserModel currentUser);
    }
}
