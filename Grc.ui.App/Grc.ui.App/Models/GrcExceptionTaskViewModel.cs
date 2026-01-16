
using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class GrcExceptionTaskViewModel {

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

    }
}
