using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class AuditSupportResponse {

        [JsonPropertyName("authorities")]
        public List<RegulatoryAuthorityResponse> Authorities { get; set; } = new();

        [JsonPropertyName("types")]
        public List<AuditMiniTypeResponse> Types { get; set; } = new();

        [JsonPropertyName("audits")]
        public List<MiniAuditResponse> Audits { get; set; } = new();

        [JsonPropertyName("responsibilities")]
        public List<ResponsibilityItemResponse> Responsibilities { get; set; } = new();

    }
}
