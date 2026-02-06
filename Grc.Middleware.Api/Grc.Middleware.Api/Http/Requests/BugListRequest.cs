using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class BugListRequest {

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

        public List<TableFilter> Filters { get; set; }
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
        public string IPAddress { get; set; }
     }
}
