using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class GrcControlMapViewModel {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("parentId")]
        public long ParentId { get; set; }

        [JsonPropertyName("include")]
        public bool Include { get; set; }

        [JsonPropertyName("control")]
        public string ControlDescription { get; set; }
    }
}
