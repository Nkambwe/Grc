using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class ReturnsDashboardResponses {

        [JsonPropertyName("periods")]
        public Dictionary<string, int> Periods { get; set; } = new();

        [JsonPropertyName("statuses")]
        public Dictionary<string, Dictionary<string, int>> Statuses { get; set; } = new();

    }

}
