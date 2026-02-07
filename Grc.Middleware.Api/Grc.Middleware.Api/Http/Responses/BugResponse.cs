using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {

    public class BugResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("error ")]
        public string Error { get; set; }

        [JsonPropertyName("source ")]
        public string Source { get; set; }

        [JsonPropertyName("stackTrace ")]
        public string StackTrace { get; set; }

        [JsonPropertyName("severity ")]
        public string Severity { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("assignedTo")]
        public string AssignedTo { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime? CreatedOn { get; set; }
    }
}
