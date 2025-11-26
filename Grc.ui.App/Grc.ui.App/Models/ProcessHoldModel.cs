using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class ProcessHoldModel {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processId")]
        public long ProcessId { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("processStatus")]
        public string ProcessStatus { get; set; }

        [JsonPropertyName("holdReason")]
        public string HoldReason { get; set; }

    }

}
