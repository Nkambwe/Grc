using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcObligationReport {

        [JsonPropertyName("statute")]
        public string Statute { get; set; }

        [JsonPropertyName("obligations")]
        public int Obligations { get; set; }

        [JsonPropertyName("compliantCount")]
        public int CompliantCount { get; set; }

        [JsonPropertyName("compliantPercentage")]
        public double CompliantPercentage { get; set; }

        [JsonPropertyName("nonCompliantCount")]
        public int NonCompliantCount { get; set; }

        [JsonPropertyName("nonCompliantPercentage")]
        public double NonCompliantPercentage { get; set; }

    }

}
