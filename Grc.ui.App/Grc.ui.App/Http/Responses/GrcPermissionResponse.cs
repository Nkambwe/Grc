using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcPermissionResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("setId")]
        public long SetId { get; set; }

        [JsonPropertyName("permissionName")]
        public string PermissionName { get; set; }

        [JsonPropertyName("isAssigned")]
        public bool IsAssigned { get; set; }

        [JsonPropertyName("permissionDescription")]
        public string PermissionDescription { get; set; }
    }
}
