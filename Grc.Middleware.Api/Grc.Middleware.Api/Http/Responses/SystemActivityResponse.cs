using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {

    public class SystemActivityResponse {

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
        public string Action {  get; set; }

        [JsonPropertyName("activityDate")]
        public DateTime ActivityDate { get; set; }

    }

}
