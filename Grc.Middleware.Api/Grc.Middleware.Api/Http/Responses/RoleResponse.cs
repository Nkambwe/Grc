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

        [JsonPropertyName("solId")]
        public string SolId { get; set; }

        [JsonPropertyName("groupId")]
        public long GroupId { get; set; }

        [JsonPropertyName("roleName")]
        public string GroupName { get; set; }

        [JsonPropertyName("deleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("verified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("approved")]
        public bool IsApproved { get; set; }

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
