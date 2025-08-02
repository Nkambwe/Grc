using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class UserByNameRequest : GrcRequest {
        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}
