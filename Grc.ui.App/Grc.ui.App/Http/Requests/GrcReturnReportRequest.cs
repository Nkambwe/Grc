using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcReturnReportRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("returnName")]
        public string ReturnName { get; set; }

        [JsonPropertyName("typeId")]
        public long TypeId { get; set; }

        [JsonPropertyName("frequencyId")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("statuteId")]
        public long StatuteId { get; set; }

        [JsonPropertyName("risk")]
        public string Risk { get; set; }

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

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
    }

}
