using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class ReturnReportResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("articleNo")]
        public string ArticleNo { get; set; }

        [JsonPropertyName("articleSummery")]
        public string ArticleSummery { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("authority")]
        public string Authority { get; set; }

        [JsonPropertyName("breachRisk")]
        public string BreachRisk { get; set; }
    }
}
