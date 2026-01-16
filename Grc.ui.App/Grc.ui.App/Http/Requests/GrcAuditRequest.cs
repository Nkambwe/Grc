using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcAuditRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("auditName")]
        public string AuditName { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("typeId")]
        public long TypeId { get; set; }

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
