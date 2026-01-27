using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class ReportRequest {
        /// <summary>
        /// Get or Set report filter
        /// </summary>
        [JsonPropertyName("filter")]
        public string Filter { get; set; }
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
        public string IpAddress { get; set; }
    }

}
