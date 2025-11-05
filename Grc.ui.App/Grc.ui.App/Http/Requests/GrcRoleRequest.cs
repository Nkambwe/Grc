using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcRoleRequest
    {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("roleName")]
        public string RoleName { get; set; }

        [JsonPropertyName("roleDescription")]
        public string RoleDescription { get; set; }

        [JsonPropertyName("groupId")]
        public long GroupId { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("isApproved")]
        public bool IsApproved { get; set; }

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }

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

        [JsonPropertyName("users")]
        public List<long> Users { get; set; } = new();

    }
}
