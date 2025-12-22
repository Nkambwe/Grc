using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class PolicySupportResponse {

        [JsonPropertyName("frequencies")]
        public List<FrequencyResponse> Frequencies { get; set; } = new();

        [JsonPropertyName("departments")]
        public List<PolicyDepartmentResponse> Departments { get; set; } = new();

        [JsonPropertyName("authorities")]
        public List<RegulatoryAuthorityResponse> Authorities { get; set; } = new();

        [JsonPropertyName("responsibilities")]
        public List<ResponsibilityItemResponse> Responsibilities { get; set; } = new();

        [JsonPropertyName("regulatoryTypes")]
        public List<RegulatoryTypeResponse> RegulatoryTypes { get; set; } = new();

    }

}
