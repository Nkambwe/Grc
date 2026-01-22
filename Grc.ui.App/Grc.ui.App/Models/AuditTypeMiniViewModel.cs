using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class AuditTypeMiniViewModel {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("typeName")]
        public string TypeName { get; set; }
    }
}
