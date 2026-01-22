using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {

    public class GrcAuditSupportResponse {

        [JsonPropertyName("authorities")]
        public List<GrcRegulatoryAuthorityResponse> Authorities { get; set; } = new();

        [JsonPropertyName("types")]
        public List<GrcAuditMiniTypeResponse> Types { get; set; } = new();

        [JsonPropertyName("audits")]
        public List<GrcMiniAuditResponse> Audits { get; set; } = new();

        [JsonPropertyName("responsibilities")]
        public List<GrcResponsibilityItemResponse> Responsibilities { get; set; } = new();

    }

}
