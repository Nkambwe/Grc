
using System.Text.Json.Serialization;

namespace Grc.ui.App.Dtos {
    public class GrcAuditUpdateResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("reportId")]
        public long ReportId { get; set; }

        [JsonPropertyName("updateNotes")]
        public string UpdateNotes { get; set; }

        [JsonPropertyName("sendReminders")]
        public bool SendReminders { get; set; }

        [JsonPropertyName("sendDate")]
        public DateTime? SendDate { get; set; }

        [JsonPropertyName("reminderMessage")]
        public string ReminderMessage { get; set; }

        [JsonPropertyName("sendToEmails")]
        public string SendToEmails { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}
