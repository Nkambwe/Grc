using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {

    public class GrcPermissionSetResponse {

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
        public List<GrcPermissionResponse> Permissions { get; set; } = new();

        [JsonPropertyName("roles")]
        public List<GrcRoleResponse> Roles { get; set; } = new();

        [JsonPropertyName("roleGroups")]
        public List<GrcRoleGroupResponse> RoleGroups { get; set; } = new();
    }
}
