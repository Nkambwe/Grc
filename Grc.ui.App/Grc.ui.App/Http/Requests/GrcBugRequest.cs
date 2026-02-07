using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcBugRequest {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; } = "";

        [JsonPropertyName("source")]
        public string Source { get; set; } = "";

        [JsonPropertyName("severity")]
        public string Severity { get; set; } = "";

        [JsonPropertyName("status")]
        public string Status { get; set; } = "";

        [JsonPropertyName("statckTrace")]
        public string StatckTrace { get; set; } = "";

        [JsonPropertyName("assignedTo")]
        public string AssignedTo { get; set; } = "";

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
    }

}
