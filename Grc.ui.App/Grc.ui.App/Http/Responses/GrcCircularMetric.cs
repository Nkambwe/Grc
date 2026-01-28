using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {

    public class GrcCircularMetric {

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("percentage")]
        public double Percentage { get; set; }
    }

}
