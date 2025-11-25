using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class ProcessReviewModel {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processStatus")]
        public string ProcessStatus { get; set; }

        [JsonPropertyName("unlockReason")]
        public string UnlockReason { get; set; }

    }

}
