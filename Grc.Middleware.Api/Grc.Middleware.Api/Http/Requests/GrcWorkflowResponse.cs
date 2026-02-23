using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class GrcWorkflowResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; }
        
        [JsonPropertyName("stepId")]
        public long StepId { get; set; }

        public List<GrcWorkflowActionRequest> Actions {get;set;}=new();
    }
}
