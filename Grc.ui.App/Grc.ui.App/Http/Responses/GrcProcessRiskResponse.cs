using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcProcessRiskResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processId")]
        public long ProcessId { get; set; }
        
        [JsonPropertyName("isCurrent")]
        public bool IsCurrent { get; set; }

        [JsonPropertyName("riskLevel")]
        public string RiskLevel { get; set; }
        
        [JsonPropertyName("impact")]
        public int Impact { get; set; }

        [JsonPropertyName("liklyhood")]
        public int Liklyhood { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

    }
}
