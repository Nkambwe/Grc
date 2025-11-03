using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {

    public class RoleGroupViewModel {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("groupName")]
        public string GroupName { get; set; }

        [JsonPropertyName("groupDescription")]
        public string GroupDescription { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }

        [JsonPropertyName("groupScope")]
        public string GroupScope { get; set; }

        [JsonPropertyName("attachedTo")]
        public string AttachedTo { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("isApproved")]
        public bool IsApproved { get; set; }

        [JsonPropertyName("permissionSets")]
        public List<long> PermissionSets { get; set; }

        [JsonPropertyName("roles")]
        public List<long> Roles { get; set; }
    }

}
