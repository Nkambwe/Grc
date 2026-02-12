using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {

    public class PasswordConfigurationModel {
        [JsonPropertyName("expirePassword")]
        public bool ExpirePassword { get; set; }
        [JsonPropertyName("exipryDays")]
        public int ExipryDays { get; set; } 
        [JsonPropertyName("minimumLength")]
        public int MinimumLength { get; set; }
        [JsonPropertyName("allowMaualReset")]
        public bool AllowMaualReset { get; set; }
        [JsonPropertyName("allowPwsReuse")]
        public bool AllowPwsReuse { get; set; }
        [JsonPropertyName("includeUpper")]
        public bool IncludeUpper { get; set; }
        [JsonPropertyName("includeLower")]
        public bool IncludeLower { get; set; }
        [JsonPropertyName("includeSpecial")]
        public bool IncludeSpecial { get; set; }
        [JsonPropertyName("includeNumerics")]
        public bool IncludeNumerics { get; set; }
        
    }
}
