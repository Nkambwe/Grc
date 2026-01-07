using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcReturnReportResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }
    }
}
