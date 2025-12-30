using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class ControlItemViewModel {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("controlName")]
        public string ControlName { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("isExcluded")]
        public bool IsExcluded { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}
