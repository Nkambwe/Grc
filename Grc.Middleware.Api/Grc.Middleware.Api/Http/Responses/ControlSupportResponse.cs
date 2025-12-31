using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ControlSupportResponse {

        [JsonPropertyName("controls")]
        public List<ControlListResponse> Controls { get; set; } = new();

        [JsonPropertyName("responsibilities")]
        public List<ResponsibilityItemResponse> Responsibilities { get; set; } = new();

    }
}
