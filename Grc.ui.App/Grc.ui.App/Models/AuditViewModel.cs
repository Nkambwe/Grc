using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class AuditViewModel {

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

    }

}
