using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class PolicyTaskResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("documentName")]
        public string PolicyDocument { get; set; }

        [JsonPropertyName("taskDescription")]
        public string TaskDescription { get; set; }

        [JsonPropertyName("taskStatus")]
        public string TaskStatus { get; set; }

        [JsonPropertyName("assigneeName")]
        public string AssigneeName { get; set; }

        [JsonPropertyName("assigneeEmail")]
        public string AssigneeEmail { get; set; }

        [JsonPropertyName("assigneeContact")]
        public string AssigneeContact { get; set; }

        [JsonPropertyName("assigneeDepartment")]
        public string AssigneeDepartment { get; set; }

        [JsonPropertyName("assigneePosition")]
        public string AssigneePosition { get; set; }

        [JsonPropertyName("assignedBy")]
        public string AssignedBy { get; set; }

        [JsonPropertyName("assignDate")]
        public DateTime AssignDate { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("lastReminder")]
        public DateTime? LastReminder { get; set; }

        [JsonPropertyName("reminderInterval")]
        public int ReminderIntervalDays { get; set; }

        [JsonPropertyName("nextReminder")]
        public DateTime? NextReminder { get; set; }

        [JsonPropertyName("reminderSent")]
        public bool LastReminderSent { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }
    }
}
