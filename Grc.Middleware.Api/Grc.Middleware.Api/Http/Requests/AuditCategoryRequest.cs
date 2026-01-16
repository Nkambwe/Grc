using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class AuditCategoryRequest {

        [JsonPropertyName("reportId")]
        public long ReportId { get; set; } = 0;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("searchTerm")]
        public string SearchTerm { get; set; }

        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("sortBy")]
        public string SortBy { get; set; }

        [JsonPropertyName("sortDirection")]
        public string SortDirection { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

    }
}
