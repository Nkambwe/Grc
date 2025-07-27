using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class UsernameValidationRequest : GrcRequest {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
    }
}
