using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class ChangePasswordModel {
        [JsonPropertyName("userId")]
        public long UserId { get; set; } = 0;

        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [JsonPropertyName("oldPassword")]
        public string OldPassword { get; set; }

        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }

        [JsonPropertyName("confirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}
