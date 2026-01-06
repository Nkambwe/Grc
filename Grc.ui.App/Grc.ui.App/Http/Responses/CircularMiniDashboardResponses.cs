using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class CircularMiniDashboardResponses {

        [JsonPropertyName("circulars")]
        public Dictionary<string, int> Circulars { get; set; } = new();

    }

}
