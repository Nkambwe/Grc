using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class AuditMiniViewModel {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("auditName")]
        public string AuditName { get; set; }
    }
}
