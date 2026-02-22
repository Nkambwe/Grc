using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class WorkflowResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; }
        
        [JsonPropertyName("stepId")]
        public long StepId { get; set; }

        public List<WorkflowActionResponse> Actions {get;set;}=new();
    }
}
