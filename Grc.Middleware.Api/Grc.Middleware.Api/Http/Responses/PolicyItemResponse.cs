using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {

    public class PolicyItemResponse {
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
