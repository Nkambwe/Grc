using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class BugViewModel {
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

        [JsonPropertyName("stackTrace")]
        public string StackTrace { get; set; } = "";

        [JsonPropertyName("assignedTo")]
        public string AssignedTo { get; set; } = "";
    }
}
