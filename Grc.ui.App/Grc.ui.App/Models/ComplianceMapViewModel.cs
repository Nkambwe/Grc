using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class ComplianceMapViewModel {
        [JsonPropertyName("sectionId")]
        public long SectionId { get; set; }

        [JsonPropertyName("categoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("controlItemIds")]
        public List<long> ControlItemIds { get; set; } = new(); 
    }
}
