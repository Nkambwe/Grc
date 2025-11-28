using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcUnitStatisticRequest {
        /// <summary>
        /// Get or Set Unit name
        /// </summary>
        [JsonPropertyName("unitName")]
        public string UnitName { get; set; }
        /// <summary>
        /// Get or Set ID of user sending request
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
        /// <summary>
        /// Get or Set Intended action
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; }

    }
}