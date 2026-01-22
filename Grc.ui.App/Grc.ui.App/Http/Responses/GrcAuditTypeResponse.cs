using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcAuditTypeResponse {

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
        public List<GrcAuditResponse> Audits { get; set; } = new();
    }

}
