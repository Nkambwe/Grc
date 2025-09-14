using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class DepartmentUnitRequest {
        /// <summary>
        /// Get or Set ID for the unit
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
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
        /// Get or Set Intended action
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; }
        /// <summary>
        /// Get or Set unit code
        /// </summary>
        [JsonPropertyName("unitCode")]
        public string UnitCode { get; set; }
        /// <summary>
        /// Get or Set unit name
        /// </summary>
        [JsonPropertyName("unitName")]
        public string UnitName { get; set; }
        /// <summary>
        /// Get or Set ID of department
        /// </summary>
        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }
        /// <summary>
        /// Get or Set whether unit is deleted
        /// </summary>
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

    }
}
