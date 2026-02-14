using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {

    public class GrcUserAccountConfigurationsRequest {

        [JsonPropertyName("canVerifySame")]
        public bool CanVerifySame { get; set; }

        [JsonPropertyName("canApproveSame")]
        public bool CanApproveSame { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
    }
}
