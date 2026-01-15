using System.Text.Json.Serialization;

namespace Grc.ui.App.Helpers {

    public class TableListRequest {

        [JsonPropertyName("userId")]
        public long UserId { get; set; } = 0;

        [JsonPropertyName("activityUserId")]
        public long? ActivityUserId { get; set; }

        [JsonPropertyName("activityTypeId")]
        public long? ActivityTypeId { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; } = "";

        [JsonPropertyName("searchTerm")]
        public string SearchTerm { get; set; } = "";

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; } = "";

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SortBy { get; set; } = "";

        public string SortDirection { get; set; } = "Ascending";

        public bool IncludeDeleted { get; set; } = false;

        [JsonPropertyName("encrypts")]
        public string[] EncryptFields { get; set; } = Array.Empty<string>();

        [JsonPropertyName("decrypts")]
        public string[] DecryptFields { get; set; } = Array.Empty<string>();
    }
}
