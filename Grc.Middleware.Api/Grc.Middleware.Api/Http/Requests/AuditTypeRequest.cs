using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class AuditTypeRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("typeCode")]
        public string TypeCode { get; set; }

        [JsonPropertyName("typeName")]
        public string TypeName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
    }

}
