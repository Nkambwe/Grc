using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests
{
    public class GrcRoleGroupRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("groupName")]
        public string GroupName { get; set; }

        [JsonPropertyName("groupDescription")]
        public string GroupDescription { get; set; }

        [JsonPropertyName("department")]
        public string DepartmentName { get; set; }

        [JsonPropertyName("groupCategory")]
        public string GroupCategory { get; set; } = "ADMINSUPPORT";

        [JsonPropertyName("groupScope")]
        public int GroupScope { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("isApproved")]
        public bool IsApproved { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

        [JsonPropertyName("permissionSets")]
        public List<long> PermissionSets { get; set; }

        [JsonPropertyName("roles")]
        public List<long> Roles { get; set; }

    }
}
