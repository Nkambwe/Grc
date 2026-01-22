using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class StatueListRequest {
        /// <summary>
        /// Get or Set ID of user activity is related to
        /// </summary>
        [JsonPropertyName("activityTypeId")]
        public long? ActivityTypeId { get; set; }
        /// <summary>
        /// Get or Set Intended action
        /// </summary>
        [JsonPropertyName("searchTerm")]
        public string SearchTerm { get; set; }
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
