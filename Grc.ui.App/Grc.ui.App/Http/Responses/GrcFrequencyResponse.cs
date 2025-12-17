using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcFrequencyResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("frequencyName")]
        public string FrequencyName { get; set; }
    }
}
