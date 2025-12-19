using System.Text.Json.Serialization;

namespace Grc.ui.App.Helpers {

    public class TableListRequest {
        /// <summary>
        /// Get or Set ID of user sending request
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        /// <summary>
        /// Get or Set ID of user activity is related to
        /// </summary>
        [JsonPropertyName("activityUserId")]
        public long? ActivityUserId { get; set; }
        /// <summary>
        /// Get or Set ID of user activity is related to
        /// </summary>
        [JsonPropertyName("activityTypeId")]
        public long? ActivityTypeId { get; set; }
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
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
        /// <summary>
        /// Get or Set Page Index
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// Get or Set Page size
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Get or Set sort parameter
        /// </summary>
        public string SortBy { get; set; }
        /// <summary>
        /// Get or Set sort direction
        /// </summary>
        public string SortDirection { get; set; }
        /// <summary>
        /// Get or Set flag whether to include deleted items
        /// </summary>
        public bool IncludeDeleted { get; set; }
        /// <summary>
        /// Get Or Send Fields intended to be encrypted
        /// </summary>
        [JsonPropertyName("encrypts")]
        public string[] EncryptFields { get; set; }
        /// <summary>
        /// Get Or Set Fields intended to be decrypted
        /// </summary>
        [JsonPropertyName("decrypts")]
        public string[] DecryptFields { get; set; }
    }
}
