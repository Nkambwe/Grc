namespace Grc.ui.App.Http.Responses
{
    public class PolicyTaskResponse
    {
        public long Id { get; set; }
        public string PolicyDocument { get; set; }
        public string TaskDescription { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedBy { get; set; }
        public string ForwardedBy { get; set; }
        public string ForwardedTo { get; set; }
        public string ForwardedMessage { get; set; }
        public DateTime AssignDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? LastReminder { get; set; }
        public int ReminderIntervalDays { get; set; }
        public DateTime? NextReminder { get; set; }
        public bool LastReminderSent { get; set; }

    }
}
