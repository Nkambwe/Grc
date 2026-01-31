using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcConfigurationParamRequest {
        /// <summary>
        /// Get or Set configuration Parameter
        /// </summary>
        [JsonPropertyName("paramName")]
        public string ParamName { get; set; }
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
        public string IPAddress { get; set; }

    }

}
