using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcCircularReportResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("authorityAlias")]
        public string AuthorityAlias { get; set; }

        [JsonPropertyName("authority")]
        public string Authority { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }
    }
}
