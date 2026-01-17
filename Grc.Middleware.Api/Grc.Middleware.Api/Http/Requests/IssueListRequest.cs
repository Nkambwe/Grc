using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class IssueListRequest {
        /// <summary>
        /// Get or Set ID of user sending request
        /// </summary>
        [JsonPropertyName("articleId")]
        public long ArticleId { get; set; }
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
        /// Get or Set Intended action
        /// </summary>
        [JsonPropertyName("searchTerm")]
        public string SearchTerm { get; set; }
        /// <summary>
        /// Get or Set User IP Address
        /// </summary>
        /// <summary>
        /// Get or Set User IP Address
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
        /// <summary>
        /// Get or Set Page Index
        /// </summary>
        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; }
        /// <summary>
        /// Get or Set Page size
        /// </summary>
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
        /// <summary>
        /// Get or Set sort parameter
        /// </summary>
        [JsonPropertyName("sortBy")]
        public string SortBy { get; set; }
        /// <summary>
        /// Get or Set sort direction
        /// </summary>
        [JsonPropertyName("sortDirection")]
        public string SortDirection { get; set; }
    }
}
