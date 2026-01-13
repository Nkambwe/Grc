using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcFrequencyResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("frequencyName")]
        public string FrequencyName { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("returns")]
        public List<ReturnReportResponse> Returns { get; set; } = new();
    }
}
