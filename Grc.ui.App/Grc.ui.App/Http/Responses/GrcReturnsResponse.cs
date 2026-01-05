using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcReturnsResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("reportName")]
        public string ReportName { get; set; }

        [JsonPropertyName("frequency")]
        public string Frequency { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("authority")]
        public string Authority { get; set; }

        [JsonPropertyName("article")]
        public string Article { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

    }
}
