using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcControlSupportResponse {

        [JsonPropertyName("controls")]
        public List<GrcControlListResponse> Controls { get; set; } = new();

        [JsonPropertyName("responsibilities")]
        public List<GrcResponsibilityItemResponse> Responsibilities { get; set; } = new();

    }
}
