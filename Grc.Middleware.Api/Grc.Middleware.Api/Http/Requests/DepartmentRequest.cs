using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class DepartmentRequest {
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
        /// Get or Set department code
        /// </summary>
        [JsonPropertyName("departmentCode")]
        public string DepartmentCode { get; set; }
        /// <summary>
        /// Get or Set department name
        /// </summary>
        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }
        /// <summary>
        /// Get or Set department alias
        /// </summary>
        [JsonPropertyName("alias")]
        public string Alias { get; set; }
        /// <summary>
        /// Get or Set ID of department
        /// </summary>
        [JsonPropertyName("branchId")]
        public long BranchId { get; set; }
        /// <summary>
        /// Get or Set whether unit is deleted
        /// </summary>
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

    }
}
