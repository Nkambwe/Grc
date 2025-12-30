using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ObligationControlMapResultResponse {

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

        [JsonPropertyName("isTested")]
        public bool IsTested { get; set; }

        [JsonPropertyName("passed")]
        public int Passed { get; set; }

        [JsonPropertyName("failed")]
        public int Failed { get; set; }

        [JsonPropertyName("issues")]
        public int Issues { get; set; }

        [JsonPropertyName("weight")]
        public decimal Weight { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

    }
}
