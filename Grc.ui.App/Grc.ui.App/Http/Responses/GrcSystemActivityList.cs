using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcSystemActivityList {
        
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("entityName")]
        public string EntityName { get; set; }

        [JsonPropertyName("activityType")]
        public string ActivityType { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("activityDate")]
        public DateTime ActivityDate { get; set; }

    }

}
