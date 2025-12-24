using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class IdRequest {
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
        public bool markAsDeleted { get; set; }
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

    public class ObligationMapRequest {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("isMandatory")]
        public bool IsMandatory { get; set; }

        [JsonPropertyName("coverage")]
        public decimal Coverage { get; set; }

        [JsonPropertyName("isCovered")]
        public bool IsCovered { get; set; }

        [JsonPropertyName("exclude")]
        public bool Exclude { get; set; }

        [JsonPropertyName("assurance")]
        public decimal Assurance { get; set; }

        [JsonPropertyName("rationale")]
        public string Rationale { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("complianceMaps")]
        public List<ComplianceMapRequest> ComplianceMaps { get; set; } = new();
    }
}
