using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {

    public class TableListRequest {

        [JsonPropertyName("userId")]
        public long UserId { get; set; } = 0;

        [JsonPropertyName("activityUserId")]
        public long? ActivityUserId { get; set; } = 0;

        [JsonPropertyName("activityTypeId")]
        public long? ActivityTypeId { get; set; } = 0;

        [JsonPropertyName("action")]
        public string Action { get; set; } = "";

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

        [JsonPropertyName("includeDeleted")]
        public bool IncludeDeleted { get; set; } = false;

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; } = "";

        [JsonPropertyName("encrypts")]
        public string[] EncryptFields { get; set; } = Array.Empty<string>();

        [JsonPropertyName("decrypts")]
        public string[] DecryptFields { get; set; } = Array.Empty<string>();
    }
}
