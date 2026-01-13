using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class CircularReportResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("breachRisk")]
        public string BreachRisk { get; set; }

        [JsonPropertyName("authorityAlias")]
        public string AuthorityAlias { get; set; }

        [JsonPropertyName("authority")]
        public string Authority { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

    }
}
