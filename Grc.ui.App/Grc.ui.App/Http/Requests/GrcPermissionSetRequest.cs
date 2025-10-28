using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {

    public class GrcPermissionSetRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("setName")]
        public string SetName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("ceatedOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("permissions")]
        public List<long> Permissions { get; set; } = new();

        [JsonPropertyName("roles")]
        public List<long> Roles { get; set; } = new();

        [JsonPropertyName("roleGroups")]
        public List<long> RoleGroups { get; set; } = new();
    }
}
