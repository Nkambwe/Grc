using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class AuditMiniTaskRequest {

        [JsonPropertyName("reportId")]
        public long ExceptionId { get; set; } = 0;

        [JsonPropertyName("searchTerm")]
        public string SearchTerm { get; set; } = "";

        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; } = 1;

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 10;

        [JsonPropertyName("sortBy")]
        public string SortBy { get; set; } = "";

        [JsonPropertyName("sortDirection")]
        public string SortDirection { get; set; } = "Ascending";

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

    }

}
