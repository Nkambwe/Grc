using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class GrcAuditTaskViewModel {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("taskName")]
        public string TaskName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("exceptionId")]
        public long ExceptionId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("sendReminder")]
        public bool SendReminder { get; set; }

        [JsonPropertyName("interval")]
        public string Interval { get; set; }

        [JsonPropertyName("intervalType")]
        public string IntervalType { get; set; }

        [JsonPropertyName("reminder")]
        public string Reminder { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

    }
}
