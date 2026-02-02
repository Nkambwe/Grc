using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {

    public class GrcGeneralConfigurationsRequest {
        /// <summary>
        /// Get or Set value whether records should be deleted or marked as deleted
        /// </summary>
        [JsonPropertyName("softDeleteRecords")]
        public bool SoftDeleteRecords { get; set; }
        /// <summary>
        /// Get or Set value whether to include deleted records in fetch results
        /// </summary>
        [JsonPropertyName("includeDeletedRecord")]
        public bool IncludeDeletedRecord { get; set; }
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
