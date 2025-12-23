using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcObligationResponse {

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
        public virtual List<GrcObligationLawResponse> Laws { get; set; } = new();

    }
}
