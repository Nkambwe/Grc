using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class ReturnsRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("reportName")]
        public string ReportName { get; set; }

        [JsonPropertyName("frequencyid")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("deadlineOn")]
        public DateTime DeadlineOn { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("articleId")]
        public long Article { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
