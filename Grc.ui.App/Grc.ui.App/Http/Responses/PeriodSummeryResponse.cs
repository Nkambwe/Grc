using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class PeriodSummeryResponse {

        [JsonPropertyName("period")]
        public string Period { get; set; }

        [JsonPropertyName("submitted")]
        public int Submitted { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("onTime")]
        public int OnTime { get; set; }

        [JsonPropertyName("breached")]
        public int Breached { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
