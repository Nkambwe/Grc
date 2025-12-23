
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ObligationLawResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("lawName")]
        public string LawName { get; set; }

        [JsonPropertyName("coverage")]
        public decimal Coverage { get; set; }

        [JsonPropertyName("isCovered")]
        public bool IsCovered { get; set; }

        [JsonPropertyName("assurance")]
        public decimal Assurance { get; set; }

        [JsonPropertyName("issues")]
        public int Issues { get; set; }

        [JsonPropertyName("sections")]
        public virtual List<ObligationActResponse> Sections { get; set; } = new();

    }
}
