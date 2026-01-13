using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class CircularSubmissionViewModel {

        [JsonPropertyName("circularId")]
        public long CircularId { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("submissionBreach")]
        public string SubmissionBreach { get; set; }

        [JsonPropertyName("circularStatus")]
        public string Status { get; set; }

        [JsonPropertyName("isBreached")]
        public bool IsBreached { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; }

        [JsonPropertyName("breachReason")]
        public string BreachReason { get; set; }

        [JsonPropertyName("submittedOn")]
        public DateTime SubmittedOn { get; set; }

        [JsonPropertyName("submittedBy")]
        public string SubmittedBy { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

    }

}
