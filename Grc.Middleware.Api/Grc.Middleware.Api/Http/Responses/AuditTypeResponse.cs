using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class AuditTypeResponse {

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

        [JsonPropertyName("audits")]
        public List<AuditResponse> Audits { get; set; } = new();
    }
}
