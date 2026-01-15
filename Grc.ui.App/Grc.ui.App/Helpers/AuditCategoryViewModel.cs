using System.Text.Json.Serialization;

namespace Grc.ui.App.Helpers {
    public class AuditCategoryViewModel {

        [JsonPropertyName("reportId")]
        public long ReportId { get; set; } = 0;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

    }
}
