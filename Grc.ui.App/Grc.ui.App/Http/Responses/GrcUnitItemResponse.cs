using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcUnitItemResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("unitName")]
        public string UnitName { get; set; }
    }
}
