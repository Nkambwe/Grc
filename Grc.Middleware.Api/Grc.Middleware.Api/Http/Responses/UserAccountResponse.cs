using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class UserAccountResponse {
        [JsonPropertyName("canApproveSame")]
        public bool CanApproveSame { get; set; } = true;
        [JsonPropertyName("canVerifySame")]
        public bool CanVerifySame { get; set; } = true;
    }

}
