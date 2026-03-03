using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcManagerReviewRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
        
        [JsonPropertyName("processId")]
        public long ProcessId { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("fileVersion")]
        public string FileVersion { get; set; }

        [JsonPropertyName("managerComments")]
        public string ManagerComments { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
     }

}
