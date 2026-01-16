using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcAuditExtensionExceptionRequest {
        /// <summary>
        /// Get or Set period to look for
        /// </summary>
        [JsonPropertyName("period")]
        public string Period { get; set; }
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
