
using System.Text.Json.Serialization;

namespace Grc.ui.App.Dtos {
    public class GrcAuditTaskResponse {

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

        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("exceptionId")]
        public long ExceptionId { get; set; }

        [JsonPropertyName("exception")]
        public string Exception { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}
