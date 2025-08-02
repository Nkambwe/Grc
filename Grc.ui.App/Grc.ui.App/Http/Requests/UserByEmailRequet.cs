using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class UserByEmailRequet : GrcRequest {
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
