using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class ApproveUserRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("isApproved")]
        public bool IsApproved { get; set; }

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
        public string IPAddress { get; set; }
        
    }
}
