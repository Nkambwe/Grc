using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class ComplianceExtensionReturnResponse {

        [JsonPropertyName("periods")]
        public Dictionary<string, int> Periods { get; set; }

        [JsonPropertyName("reports")]
        public List<GrcReturnSubmissionResponse> Reports { get; set; }
    }

}
