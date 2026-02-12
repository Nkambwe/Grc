using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class PasswordConfigurationsRequest {
        [JsonPropertyName("enforcePasswordExpiration")]
        public bool EnforcePasswordExpiration { get; set; }
        [JsonPropertyName("daysUntilPasswordExpiration")]
        public int DaysUntilPasswordExpiration { get; set; }
        [JsonPropertyName("minimumPasswordLength")]
        public int MinimumPasswordLength { get; set; }
        [JsonPropertyName("allowManualPasswordReset")]
        public bool AllowManualPasswordReset { get; set; }
        [JsonPropertyName("allowPasswordReuse")]
        public bool AllowPasswordReuse { get; set; }
        [JsonPropertyName("includeUppercaseCharacters")]
        public bool IncludeUppercaseCharacters { get; set; }
        [JsonPropertyName("includeLowercaseCharacters")]
        public bool IncludeLowercaseCharacters { get; set; }
        [JsonPropertyName("includeSpecialCharacters")]
        public bool IncludeSpecialCharacters { get; set; }
        [JsonPropertyName("includeNumericCharacters")]
        public bool IncludeNumericCharacters { get; set; }
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("action")]
        public string Action { get; set; }
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
    }
}
