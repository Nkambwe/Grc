using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {

    public class RegulatoryCategoryResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("updatedOn")]
        public DateTime UpdatedOn { get; set; }

        [JsonPropertyName("statutes")]
        public List<StatutoryRegulationResponse> Statutes { get; set; } = new();
    }

}
