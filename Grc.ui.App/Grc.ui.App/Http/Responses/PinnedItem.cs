using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class PinnedItem {
        [JsonPropertyName("id")]
        public long Id{ get; set;}
        [JsonPropertyName("label")]
        public string Label{ get; set;}
        [JsonPropertyName("icon")]
        public string IconClass{ get; set;}
        [JsonPropertyName("controller")]
        public string Controller{ get; set;}
        [JsonPropertyName("action")]
        public string Action{ get; set;}
        [JsonPropertyName("area")]
        public string Area{ get; set;}
        [JsonPropertyName("cssClass")]
        public string CssClass{ get; set;}
    }
}
