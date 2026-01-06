using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class ComplianceTaskMinStatisticResponse {

        [JsonPropertyName("tasks")]
        public Dictionary<string, int> Tasks { get; set; } = new();

    }
}
