namespace Grc.ui.App.Models
{
    public class PolicyTaskViewModel {
        public long Id { get; set; }
        public string PolicyId { get; set; }
        public string TaskDescription { get; set; }
        public long AssignedTo { get; set; }
        public string AssignedBy { get; set; }
        public DateTime AssignDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? LastReminder { get; set; }
        public int ReminderIntervalDays { get; set; }
        public DateTime? NextReminder { get; set; }
        public bool LastReminderSent { get; set; }

    }
}
