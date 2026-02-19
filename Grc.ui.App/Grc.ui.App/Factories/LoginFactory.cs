using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {

    public class LoginFactory : ILoginFactory {
        private readonly ISystemConfiguration _configuration;
        protected readonly IWebHelper _webHelper;
        public LoginFactory(ISystemConfiguration configuration, IWebHelper webHelper) {
            _configuration = configuration;
            _webHelper = webHelper;
        }

        public async Task<LoginModel> PrepareLoginModelAsync() {
            var configurations = await _configuration.GetPasswordConfigurationAsync(0, _webHelper.GetCurrentIpAddress());
            GrcSecuritySettingsResponse data;
            if (configurations.HasError) {
                data = new GrcSecuritySettingsResponse();
            } else {
                data = configurations.Data;
            }

            return new LoginModel() {
                Username = string.Empty,
                Password = string.Empty,
                RememberMe = false,
                PasswordSettings = data
            };
        }

        public Task<UsernameValidationModel> PrepareUsernameValidationModelAsync(string username, string ipAddress) {
                return Task.FromResult(new UsernameValidationModel() {
                    Username = username,
                    IPAddress = ipAddress,
                });
        }

        public Task<LogoutModel> PrepareLogoutModelAsync(long userId, string ipAddress){ 
            return Task.FromResult(new LogoutModel() {
                UserId = userId,
                IPAddress = ipAddress,
            });
        }
    }

}
