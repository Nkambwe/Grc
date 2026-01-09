using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcStatuteSupportResponse {

        [JsonPropertyName("frequencies")]
        public List<GrcFrequencyResponse> Frequencies { get; set; } = new();

        [JsonPropertyName("departments")]
        public List<GrcDepartmentResponse> Departments { get; set; } = new();

        [JsonPropertyName("authorities")]
        public List<GrcRegulatoryAuthorityResponse> Authorities { get; set; } = new();

        [JsonPropertyName("responsibilities")]
        public List<GrcResponsibilityItemResponse> Responsibilities { get; set; } = new();

        [JsonPropertyName("statuteTypes")]
        public List<GrcStatuteTypeResponse> StatuteTypes { get; set; } = new();

    } 
}
