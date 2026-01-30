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

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("authority")]
        public string Authority { get; set; }

        [JsonPropertyName("article")]
        public string Article { get; set; }

        [JsonPropertyName("sendReminder")]
        public bool SendReminder { get; set; }

        [JsonPropertyName("interval")]
        public string Interval { get; set; }

        [JsonPropertyName("intervalType")]
        public string IntervalType { get; set; }

        [JsonPropertyName("reminder")]
        public string Reminder { get; set; }

        [JsonPropertyName("requiredSubmissionDate")]
        public DateTime? RequiredSubmissionDate { get; set; }

        [JsonPropertyName("requiredSubmissionDay")]
        public int RequiredSubmissionDay { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

    }
}
