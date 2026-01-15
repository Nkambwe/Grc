using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class AuditTaskRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("taskName")]
        public string TaskName { get; set; }

        [JsonPropertyName("taskDescription")]
        public string TaskDescription { get; set; }

        [JsonPropertyName("duedate")]
        public DateTime? Duedate { get; set; }

        [JsonPropertyName("taskStatus")]
        public string TaskStatus { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("exceptionId")]
        public long ExceptionId { get; set; }

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
