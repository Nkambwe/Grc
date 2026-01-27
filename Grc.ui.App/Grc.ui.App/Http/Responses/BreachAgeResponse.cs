using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {

    public class BreachAgeResponse {

        [JsonPropertyName("reportName")]
        public string ReportName { get; set; }

        [JsonPropertyName("authority")]
        public string Authority { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; }

        [JsonPropertyName("submissionDate")]
        public DateTime? SubmissionDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("age")]
        public int Age { get; set; }

    }

}
