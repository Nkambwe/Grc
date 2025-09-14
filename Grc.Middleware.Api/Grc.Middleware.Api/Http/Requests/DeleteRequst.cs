using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class DeleteRequst {
        /// <summary>
        /// Get or Set ID for the record to delete
        /// </summary>
        [JsonPropertyName("recordId")]
        public long RecordId { get; set; }
        /// <summary>
        /// Get or Set ID of user sending request
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        /// <summary>
        /// Get or Set value whether o mark as deleted or delete record
        /// </summary>
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
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
