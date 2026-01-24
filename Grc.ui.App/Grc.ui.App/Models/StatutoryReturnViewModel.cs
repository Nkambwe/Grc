using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class  StatutoryReturnViewModel {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("sectionId")]
        public long SectionId { get; set; }

        [JsonPropertyName("returnTypeId")]
        public long ReturnTypeId { get; set; }

        [JsonPropertyName("returnName")]
        public string ReturnName { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("frequencyId")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("riskAttached")]
        public string RiskAttached { get; set; }

        [JsonPropertyName("sendReminder")]
        public bool SendReminder { get; set; }

        [JsonPropertyName("interval")]
        public string Interval { get; set; }

        [JsonPropertyName("intervalType")]
        public string IntervalType { get; set; }

        [JsonPropertyName("reminder")]
        public string Reminder { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

    }

}
