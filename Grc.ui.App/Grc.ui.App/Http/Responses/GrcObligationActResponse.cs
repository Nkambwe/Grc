using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcObligationActResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        [JsonPropertyName("section")]
        public string Section { get; set; }

        [JsonPropertyName("requirement")]
        public string Requirement { get; set; }

        [JsonPropertyName("coverage")]
        public decimal Coverage { get; set; }

        [JsonPropertyName("isCovered")]
        public bool IsCovered { get; set; }

        [JsonPropertyName("assurance")]
        public decimal Assurance { get; set; }

        [JsonPropertyName("issues")]
        public int Issues { get; set; }
    }
}
