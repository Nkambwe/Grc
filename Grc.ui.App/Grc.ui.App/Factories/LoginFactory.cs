using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public class LoginFactory : ILoginFactory {

        public Task<LoginModel> PrepareLoginModelAsync() {
            return Task.FromResult(new LoginModel() {
                UsernameOrEmail = string.Empty,
                Password = string.Empty,
                RememberMe = false
            });
        }
    }

}
