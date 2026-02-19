using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class SecuritySettingsResponse {
        [JsonPropertyName("expirePassword")]
        public bool ExpirePassword { get; set; }
        [JsonPropertyName("exipreyPeriod")]
        public int ExipreyPeriod { get; set; }
        [JsonPropertyName("canUseOldPassword")]
        public bool CanUseOldPassword { get; set; }
        [JsonPropertyName("allowAdmininsToResetPasswords")]
        public bool AllowAdmininsToResetPasswords { get; set; }
        [JsonPropertyName("minimumPasswordLength")]
        public int MinimumPasswordLength { get; set; }
        [JsonPropertyName("includeUpperCharacters")]
        public bool IncludeUpperCharacters { get; set; }
        [JsonPropertyName("includeLowerCharacters")]
        public bool IncludeLowerCharacters { get; set; }
        [JsonPropertyName("includeSpecialCharacters")]
        public bool IncludeSpecialCharacters { get; set; }
        [JsonPropertyName("allowPasswordReuse")]
        public bool AllowPasswordReuse { get; set; }
        [JsonPropertyName("includeNumericCharacters")]
        public bool IncludeNumericCharacters { get; set; }
    }
}
