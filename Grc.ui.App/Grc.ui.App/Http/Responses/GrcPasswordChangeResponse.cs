using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcPasswordChangeResponse {
        [JsonPropertyName("minimumLength")]
        public int MinimumLength { get; set; } = 12;
        [JsonPropertyName("includeUpperChar")]
        public bool IncludeUpperChar { get; set; } = true;
        [JsonPropertyName("includeLowerChar")]
        public bool IncludeLowerChar { get; set; } = true;
        [JsonPropertyName("includeSpecialChar")]
        public bool IncludeSpecialChar { get; set; } = true;
        [JsonPropertyName("canReusePasswords")]
        public bool CanReusePasswords { get; set; } = false;
        [JsonPropertyName("includeNumericChar")]
        public bool IncludeNumericChar { get; set; } = true;
    }
}
