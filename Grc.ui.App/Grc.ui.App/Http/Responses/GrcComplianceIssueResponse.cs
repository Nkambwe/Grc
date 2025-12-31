using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {

    public class GrcComplianceIssueResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("articleId")]
        public long ArticleId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("isClosed")]
        public bool IsClosed { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }

}
