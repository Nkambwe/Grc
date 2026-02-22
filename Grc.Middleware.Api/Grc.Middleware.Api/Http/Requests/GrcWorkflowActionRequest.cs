using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class GrcWorkflowActionRequest {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("workflowId")]
        public long WorkflowId { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }
    }
}
