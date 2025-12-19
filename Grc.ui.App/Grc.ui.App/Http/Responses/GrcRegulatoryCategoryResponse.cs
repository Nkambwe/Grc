using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {

    public class GrcRegulatoryCategoryResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("updatedOn")]
        public DateTime UpdatedOn { get; set; }

        [JsonPropertyName("statutes")]
        public List<GrcStatutoryLawResponse> Statutes { get; set; } = new();
    }

}
