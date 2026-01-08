using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcReturnSubmissionResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("risk")]
        public string Risk { get; set; }
    }

}
