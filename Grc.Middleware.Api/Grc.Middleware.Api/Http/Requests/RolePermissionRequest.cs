using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class RolePermissionRequest
    {
        /// <summary>
        /// Get or Set ID for the role to look for
        /// </summary>
        [JsonPropertyName("roleId")]
        public long RoleId { get; set; }
        /// <summary>
        /// Get or Set ID of user sending request
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        /// <summary>
        /// Get or Set value whether o mark as deleted or delete record
        /// </summary>
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
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

        [JsonPropertyName("permissions")]
        public List<PermissionRequest> Permissions { get; set; } = new();
    }
}
