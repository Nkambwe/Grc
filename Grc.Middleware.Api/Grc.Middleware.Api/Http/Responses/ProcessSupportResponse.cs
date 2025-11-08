using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ProcessSupportResponse
    {
        [JsonPropertyName("processTypes")]
        public List<ProcessTypeItemResponse> ProcessTypes { get; set; } = new();

        [JsonPropertyName("units")]
        public List<UnitItemResponse> Units { get; set; } = new();

        [JsonPropertyName("responsibilities")]
        public List<ResponsibilityItemResponse> Responsibilities { get; set; } = new();
    }
}
