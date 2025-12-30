using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class ControlCategoryViewModel {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("isExcluded")]
        public bool IsExcluded { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}
