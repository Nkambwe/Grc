using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {

    public class GrcPermissionSetViewModel {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("setName")]
        public string SetName { get; set; }

        [JsonPropertyName("setDescription")]
        public string Description { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("roles")]
        public List<long> Roles { get; set; } = new();

        [JsonPropertyName("permissions")]
        public List<long> Permissions { get; set; } = new();

        [JsonPropertyName("roleGroups")]
        public List<long> RoleGroups { get; set; } = new();
    }

}
