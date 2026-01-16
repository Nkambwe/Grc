using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class AuditResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("auditName")]
        public string AuditName { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("authority")]
        public string Authority { get; set; }

        [JsonPropertyName("typeId")]
        public long TypeId { get; set; }

        [JsonPropertyName("typeName")]
        public string TypeName { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }

}
