using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcControlItemRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("itemName")]
        public string ItemName { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("exclude")]
        public bool Exclude { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("items")]
        public List<GrcControlItemRequest> ControlItems { get; set; } = new();
    }
}
