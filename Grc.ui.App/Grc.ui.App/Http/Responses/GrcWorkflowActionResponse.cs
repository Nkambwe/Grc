using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcWorkflowActionResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("workflowId")]
        public long WorkflowId { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }
    }
}
