using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class PermissionResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("permissionName")]
        public string PermissionName { get; set; }

        [JsonPropertyName("isAssigned")]
        public bool IsAssigned { get; set; }

        [JsonPropertyName("description")]
        public string PermissionDescription { get; set; }
       
    }
}
