using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {

    public class AcivityLogRequest {
        /// <summary>
        /// Get or Set ID of user sending request
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        /// <summary>
        /// Get or Set User IP Address
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }

        /// <summary>
        /// Get or Set Activity name
        /// </summary>
        [JsonPropertyName("activity")]
        public string Activity { get; set; }

        /// <summary>
        /// Get or Set comment
        /// </summary>
        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Get or Set User system keyword
        /// </summary>
        [JsonPropertyName("systemKeyword")]
        public string SystemKeyword { get; set; }

        /// <summary>
        /// Get or Set Entity name
        /// </summary>
        [JsonPropertyName("entityName")]
        public string EntityName { get; set; }
    }
}
