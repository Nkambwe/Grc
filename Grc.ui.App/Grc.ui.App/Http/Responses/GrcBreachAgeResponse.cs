using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {

    public class GrcBreachAgeResponse {

        [JsonPropertyName("reportName")]
        public string ReportName { get; set; }

        [JsonPropertyName("frequency")]
        public string Frequency { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; }

        [JsonPropertyName("submissionDate")]
        public DateTime? SubmissionDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("daysOverdue")]
        public int DaysOverdue { get; set; }

        [JsonPropertyName("agingBucket")]
        public string AgingBucket { get; set; }

        [JsonPropertyName("associatedRisk")]
        public string AssociatedRisk { get; set; }
    }

}
