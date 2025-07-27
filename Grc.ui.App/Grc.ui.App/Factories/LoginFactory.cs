using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public class LoginFactory : ILoginFactory {

        public Task<LoginModel> PrepareLoginModelAsync() {
            return Task.FromResult(new LoginModel() {
                Username = string.Empty,
                Password = string.Empty,
                RememberMe = false
            });
        }

        public Task<UsernameValidationModel> PrepareUsernameValidationModelAsync(string username, string ipAddress) {
            return Task.FromResult(new UsernameValidationModel() {
                Username = username,
                IPAddress = ipAddress,
            });
        }
    }

}
