using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcArticleRevisionResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("articleId")]
        public long ArticleId { get; set; }

        [JsonPropertyName("section")]
        public string Section { get; set; }

        [JsonPropertyName("summery")]
        public string Summery { get; set; }

        [JsonPropertyName("revision")]
        public string Revision { get; set; }

        [JsonPropertyName("reviewedOn")]
        public DateTime ReviewedOn { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("ceatedOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime ModifiedOn { get; set; }
    }
}
