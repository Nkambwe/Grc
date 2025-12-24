using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class RegulatoryViewModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("typeName")]
        public string TypeName { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }

}
