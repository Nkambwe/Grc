using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ControlMapResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("parentId")]
        public long ParentId { get; set; }

        [JsonPropertyName("include")]
        public bool Include { get; set; }

        [JsonPropertyName("controlCode")]
        public string ControlCode { get; set; }

        [JsonPropertyName("mapControl")]
        public string Description { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

    }
}
