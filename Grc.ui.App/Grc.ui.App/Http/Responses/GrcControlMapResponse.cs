using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcControlMapResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("categoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("itemName")]
        public string ItemName { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("include")]
        public bool Include { get; set; } = true;

        [JsonPropertyName("exclude")]
        public bool Exclude { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; }
    }
}
