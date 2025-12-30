using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ObligationComplianceItemResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("categoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("itemName")]
        public string ItemName { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("exclude")]
        public bool Exclude { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; }
    }
}
