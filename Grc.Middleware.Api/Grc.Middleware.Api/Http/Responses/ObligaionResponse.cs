
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ObligaionResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("coverage")]
        public decimal Coverage { get; set; }

        [JsonPropertyName("isCovered")]
        public bool IsCovered { get; set; }

        [JsonPropertyName("assurance")]
        public decimal Assurance { get; set; }

        [JsonPropertyName("issues")]
        public int Issues { get; set; }

        [JsonPropertyName("laws")]
        public virtual List<ObligationLawResponse> Laws { get; set; } = new();

    }
}
