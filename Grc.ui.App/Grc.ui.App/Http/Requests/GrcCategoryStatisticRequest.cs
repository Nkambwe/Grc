using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcCategoryStatisticRequest {
        /// <summary>
        /// Get or Set category name
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; }
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