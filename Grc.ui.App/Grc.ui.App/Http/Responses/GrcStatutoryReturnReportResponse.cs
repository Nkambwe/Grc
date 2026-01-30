using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcStatutoryReturnReportResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("returnName")]
        public string ReturnName { get; set; }

        [JsonPropertyName("send_reminder")]
        public bool SendReminder { get; set; }

        [JsonPropertyName("interval")]
        public string Interval { get; set; }

        [JsonPropertyName("interval_type")]
        public string IntervalType { get; set; }

        [JsonPropertyName("reminder")]
        public string Reminder { get; set; }

        [JsonPropertyName("typeId")]
        public long TypeId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("authority")]
        public string Authority { get; set; }

        [JsonPropertyName("frequencyId")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("frequency")]
        public string Frequency { get; set; }

        [JsonPropertyName("statuteId")]
        public long StatuteId { get; set; }

        [JsonPropertyName("statute")]
        public string Statute { get; set; }

        [JsonPropertyName("risk")]
        public string Risk { get; set; }

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
