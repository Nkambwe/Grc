using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class SubmissionViewModel {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("file")]
        public string File { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("submittedBy")]
        public string SubmittedBy { get; set; }

        [JsonPropertyName("submittedOn")]
        public DateTime SubmittedOn { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

    }

}
