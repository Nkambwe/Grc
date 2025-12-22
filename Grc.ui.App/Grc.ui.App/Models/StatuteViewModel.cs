using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class StatuteViewModel {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("lawCode")]
        public string StatuteCode { get; set; }

        [JsonPropertyName("lawName")]
        public string StatuteName { get; set; }

        [JsonPropertyName("typeId")]
        public long StatuteTypeId { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("categoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

    }
}
