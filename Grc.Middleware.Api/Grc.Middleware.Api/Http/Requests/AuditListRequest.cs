using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class AuditListRequest {
        /// <summary>
        /// Get or Set audit report id
        /// </summary>
        [JsonPropertyName("reportId")]
        public long ReportId { get; set; } = 0;
        /// <summary>
        /// Get or Set period to look for
        /// </summary>
        [JsonPropertyName("period")]
        public string Period { get; set; } = string.Empty;
        /// <summary>
        /// Get or Set Intended action
        /// </summary>
        [JsonPropertyName("searchTerm")]
        public string SearchTerm { get; set; } = string.Empty;
        /// <summary>
        /// Get or Set Page Index
        /// </summary>
        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; } = 50;
        /// <summary>
        /// Get or Set Page size
        /// </summary>
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 1;
        /// <summary>
        /// Get or Set sort parameter
        /// </summary>
        [JsonPropertyName("sortBy")]
        public string SortBy { get; set; } = string.Empty;
        /// <summary>
        /// Get or Set sort direction
        /// </summary>
        [JsonPropertyName("sortDirection")]
        public string SortDirection { get; set; } = string.Empty;
        /// <summary>
        /// Get or Set ID of user sending request
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        /// <summary>
        /// Get or Set Intended action
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; }
        /// <summary>
        /// Get or Set User IP Address
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

    }
}
