using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class PolicyTaskViewModel {
        public long Id { get; set; }
        public long PolicyId { get; set; }
        public string PolicyDocument { get; set; }
        public string DocumentComments { get; set; }
        public string TaskDescription { get; set; }
        public long AssignedTo { get; set; }
        public string AssigneeName { get; set; }
        public string AssigneeEmail { get; set; }
        public string AssigneeContact { get; set; }
        public string AssigneeDepartment { get; set; }
        public string AssigneePosition { get; set; }
        public string AssignedBy { get; set; }
        public DateTime AssignDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? LastReminder { get; set; }
        public int ReminderIntervalDays { get; set; }
        public DateTime? NextReminder { get; set; }
        public bool LastReminderSent { get; set; }

        public long UserId { get; set; }
        public string IPAddress { get; set; }
        public string Action { get; set; }

    }
}
