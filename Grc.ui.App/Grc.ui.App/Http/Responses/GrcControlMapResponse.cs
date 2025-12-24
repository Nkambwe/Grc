using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcControlMapResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("parentId")]
        public long ParentId { get; set; }

        [JsonPropertyName("include")]
        public bool Include { get; set; }

        [JsonPropertyName("mapControl")]
        public string Control { get; set; }
    }
}
