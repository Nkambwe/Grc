using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class PasswordResetRequest
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime ModifiedOn { get; set; }
    }
}
