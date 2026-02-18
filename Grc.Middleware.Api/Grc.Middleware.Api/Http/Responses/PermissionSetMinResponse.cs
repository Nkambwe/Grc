using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class PermissionSetMinResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("setName")]
        public string SetName { get; set; }

        [JsonPropertyName("setDescription")]
        public string SetDescription { get; set; }
    }
}
