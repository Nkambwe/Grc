using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class RoleGroupPermissionRequest
    {
        /// <summary>
        /// Get or Set ID for the role to look for
        /// </summary>
        [JsonPropertyName("roleGroupId")]
        public long RoleGroupId { get; set; }
        /// <summary>
        /// Get or Set ID of user sending request
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        /// <summary>
        /// Get or Set value whether to include permissions
        /// </summary>
        [JsonPropertyName("includePermissions")]
        public bool IncludePermissions { get; set; }
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
        public List<PermissionSetRequest> PermissionSets { get; set; } = new List<PermissionSetRequest>();
    }
}
