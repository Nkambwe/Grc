using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {

    public class GrcProcessSupportResponse {

        [JsonPropertyName("processTypes")]
        public List<GrcProcessTypeItemResponse> ProcessTypes { get; set; } = new();

        [JsonPropertyName("units")]
        public List<GrcUnitItemResponse> Units { get; set; } = new();

        [JsonPropertyName("responsibilities")]
        public List<GrcResponsibilityItemResponse> Responsibilities { get; set; } = new();
    }
}
