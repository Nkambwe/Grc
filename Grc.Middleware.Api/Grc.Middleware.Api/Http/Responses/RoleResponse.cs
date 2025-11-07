using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class RoleResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("roleName")]
        public string RoleName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("groupId")]
        public long GroupId { get; set; }

        [JsonPropertyName("groupName")]
        public string GroupName { get; set; }

        [JsonPropertyName("groupCategory")]
        public string GroupCategory { get; set; }

        [JsonPropertyName("deleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("verified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("approved")]
        public bool IsApproved { get; set; }

        [JsonPropertyName("isAssigned")]
        public bool IsAssigned { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("ceatedOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

        [JsonPropertyName("permissions")]
        public List<PermissionResponse> Permissions { get; set; } = new();

        [JsonPropertyName("permissionsets")]
        public List<PermissionSetResponse> PermissionSets { get; set; } = new();

        [JsonPropertyName("users")]
        public List<UserResponse> Users { get; set; } = new();

    }
}
