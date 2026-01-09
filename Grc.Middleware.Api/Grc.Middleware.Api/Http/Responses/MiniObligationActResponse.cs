using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class MiniObligationActResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("section")]
        public string Section { get; set; }

        [JsonPropertyName("requirement")]
        public string Requirement { get; set; }
    }
}
