using Grc.ui.App.Dtos;
using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class AuditTypeViewModel {

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

    }
}
