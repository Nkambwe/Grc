
using Grc.ui.App.Enums;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Models {

    public class LoginModel {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public bool IsUsernameValidated { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public string DisplayName { get; set; } = string.Empty;
        public GrcSecuritySettingsResponse PasswordSettings { get; set; } = new();
        public LoginStage CurrentStage { get; set; } = LoginStage.Username;
    }
}
