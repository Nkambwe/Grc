using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcRoleGroupResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("groupName")]
        public string GroupName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("groupCategory")]
        public string GroupCategory { get; set; }

        [JsonPropertyName("groupScope")]
        public string GroupScope { get; set; }

        [JsonPropertyName("groupType")]
        public string GroupType { get; set; }

        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }

        [JsonPropertyName("deleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("verified")]
        public bool? IsVerified { get; set; }

        [JsonPropertyName("approved")]
        public bool? IsApproved { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("ceatedOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

    }
}
