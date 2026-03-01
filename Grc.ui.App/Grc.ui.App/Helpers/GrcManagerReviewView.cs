using System.Text.Json.Serialization;

namespace Grc.ui.App.Helpers {
    public class GrcManagerReviewView {

        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        [JsonPropertyName("processId")]
        public long ProcessId { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
        
        [JsonPropertyName("fileVersion")]
        public string FileVersion { get; set; }

        [JsonPropertyName("managerComments")]
        public string ManagerComments { get; set; }
     }
}
