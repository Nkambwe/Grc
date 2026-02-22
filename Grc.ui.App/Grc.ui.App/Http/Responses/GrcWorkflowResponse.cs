using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcWorkflowResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; }
        
        [JsonPropertyName("stepId")]
        public long StepId { get; set; }

        public List<GrcWorkflowActionResponse> Actions {get;set;}=new();
    }
}
