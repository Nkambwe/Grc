using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcPeriodStatisticRequest {

        [JsonPropertyName("period")]
        public string Period { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

    }
}