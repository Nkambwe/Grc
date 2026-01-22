using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class AuditUpdateRequest {
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

        [JsonPropertyName("addedBy")]
        public string AddedBy { get; set; }

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
