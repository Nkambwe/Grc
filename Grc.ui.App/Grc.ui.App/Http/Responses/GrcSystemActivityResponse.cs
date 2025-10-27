using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {

    public class GrcSystemActivityResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("entityName")]
        public string EntityName { get; set; }

        [JsonPropertyName("activityType")]
        public string ActivityType { get; set; }

        [JsonPropertyName("accessedBy")]
        public string AccessedBy { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("activityDate")]
        public DateTime ActivityDate { get; set; }

    }

}
