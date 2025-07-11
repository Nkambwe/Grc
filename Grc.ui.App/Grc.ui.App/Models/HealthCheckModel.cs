using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {

    public class HealthCheckModel {
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("isConnected")]
        public bool IsConnected { get; set; }

        [JsonPropertyName("hasCompany")]
        public bool HasCompany { get; set; }

    }

}
