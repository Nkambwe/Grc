using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class ApproveUserViewModel {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("isApproved")]
        public bool IsApproved { get; set; }

    }
}
