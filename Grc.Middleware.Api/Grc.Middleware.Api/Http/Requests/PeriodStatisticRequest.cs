using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class PeriodStatisticRequest {

        [JsonPropertyName("period")]
        public string Period { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

    }
}
