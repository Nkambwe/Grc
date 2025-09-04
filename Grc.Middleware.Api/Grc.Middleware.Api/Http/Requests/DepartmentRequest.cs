using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class DepartmentRequest {
        /// <summary>
        /// Get or Set Id fo department
        /// </summary>
        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }
        /// <summary>
        /// Get or Set department name
        /// </summary>
        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }
        /// <summary>
        /// Get or Set department alias
        /// </summary>
        [JsonPropertyName("departmentAlias")]
        public string DepartmentAlias { get; set; }
        /// <summary>
        /// Get or Set delete status
        /// </summary>
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
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

    }
}
