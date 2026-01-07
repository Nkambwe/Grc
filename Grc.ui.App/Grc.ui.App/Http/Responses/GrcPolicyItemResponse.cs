using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcPolicyItemResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("reviewDate")]
        public DateTime ReviewDate { get; set; }
    }
}
