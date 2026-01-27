using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class PasswordViewModel {

        [JsonPropertyName("oldPassword")]
        public string OldPassword { get; set; }

        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }

        [JsonPropertyName("confirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}
