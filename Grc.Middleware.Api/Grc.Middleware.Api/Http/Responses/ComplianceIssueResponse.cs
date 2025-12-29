using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ComplianceIssueResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("articleId")]
        public long ArticleId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

    }
}
