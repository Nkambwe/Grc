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
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedOn")]
        public DateTime UpdatedAt { get; set; }
    }

}
