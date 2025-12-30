using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class ItemViewModel {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("categoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("itemName")]
        public string ItemName { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("isExcluded")]
        public bool IsExcluded { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}
