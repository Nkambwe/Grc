using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcReturnSubmissionResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("article")]
        public string Article { get; set; }

        [JsonPropertyName("report")]
        public string Report { get; set; }

        [JsonPropertyName("periodStart")]
        public DateTime PeriodStart { get; set; }

        [JsonPropertyName("periodEnd")]
        public DateTime PeriodEnd { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("requiredDate")]
        public DateTime? RequiredDate { get; set; }

        [JsonPropertyName("requiredDay")]
        public int RequiredDay { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("risk")]
        public string Risk { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("isBreached")]
        public bool IsBreached { get; set; }

        [JsonPropertyName("breachReason")]
        public string BreachReason { get; set; }

        [JsonPropertyName("submittedOn")]
        public DateTime? SubmittedOn { get; set; }

        [JsonPropertyName("submittedBy")]
        public string SubmittedBy { get; set; }

    }

}
