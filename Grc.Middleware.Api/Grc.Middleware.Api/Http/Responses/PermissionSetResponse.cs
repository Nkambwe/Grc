using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class PermissionSetResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("setName")]
        public string SetName { get; set; }

        [JsonPropertyName("setDescription")]
        public string SetDescription { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime? ModifiedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("permissions")]
        public List<PermissionResponse> Permissions { get; set; } = new();

        [JsonPropertyName("roles")]
        public List<RoleResponse> Roles { get; set; } = new();

        [JsonPropertyName("roleGroups")]
        public List<RoleGroupResponse> RoleGroups { get; set; } = new();

    }
}
