using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcMiniObligationActResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("section")]
        public string Section { get; set; }

        [JsonPropertyName("requirement")]
        public string Requirement { get; set; }
    }
}
