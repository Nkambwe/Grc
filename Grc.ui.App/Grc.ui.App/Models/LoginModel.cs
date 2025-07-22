
namespace Grc.ui.App.Models {

    public class LoginModel {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
