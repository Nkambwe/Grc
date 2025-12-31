using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcControlListResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("controlName")]
        public string ControlName { get; set; }

    }
}
