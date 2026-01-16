using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcAuditMiniUpdateRequest {

        [JsonPropertyName("reportId")]
        public long ExceptionId { get; set; } = 0;

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

    }
}