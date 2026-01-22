using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class AuditListViewModel {
        /// <summary>
        /// Get or Set audit report id
        /// </summary>
        [JsonPropertyName("reportId")]
        public long ReportId { get; set; } = 0;
        /// <summary>
        /// Get or Set report period
        /// </summary>
        [JsonPropertyName("reportPeriod")]
        public string ReportPeriod { get; set; } = string.Empty;
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

    }
}
