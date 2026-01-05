using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcReturnsRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("reportName")]
        public string ReportName { get; set; }

        [JsonPropertyName("frequencyId")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("deadlineOn")]
        public DateTime DeadlineOn { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("authorityId")]
        public string AuthorityId { get; set; }

        [JsonPropertyName("articleId")]
        public long ArticleId { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("action")]
        public string Action { get; set; }
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
    }
}
