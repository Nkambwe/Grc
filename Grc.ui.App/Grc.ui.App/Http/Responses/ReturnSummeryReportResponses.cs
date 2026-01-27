using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class ReturnSummeryReportResponses {

        [JsonPropertyName("counts")]
        public Dictionary<string, int> Counts { get; set; } = new();

        [JsonPropertyName("percentages")]
        public Dictionary<string, int> Percentages { get; set; } = new();

    }

}
