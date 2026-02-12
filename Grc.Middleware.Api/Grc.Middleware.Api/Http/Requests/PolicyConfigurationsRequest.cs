using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class PolicyConfigurationsRequest {
        /// <summary>
        /// Get or Set value whether to send policy notifications
        /// </summary>
        [JsonPropertyName("sendPolicyNotifications")]
        public bool SendPolicyNotifications { get; set; }
        /// <summary>
        /// Get or Set the maximum number of notifications to send
        /// </summary>
        [JsonPropertyName("maximumNumberOfNotifications")]
        public int MaximumNumberOfNotifications { get; set; }
        /// <summary>
        /// Get or Set ID of user sending request
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; } = 0;
        /// <summary>
        /// Get or Set Intended action
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;
        /// <summary>
        /// Get or Set User IP Address
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; } = string.Empty;

    }
}
