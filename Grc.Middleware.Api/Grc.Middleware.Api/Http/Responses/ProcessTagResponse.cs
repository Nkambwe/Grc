using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ProcessTagResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("tagName")]
        public string TagName { get; set; }

        [JsonPropertyName("tagDescription")]
        public string TagDescription { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime? ModifiedOn { get; set; }

        [JsonPropertyName("processes")]
        public List<ProcessRegisterResponse> Processes { get; set; } = new();

    }
}
