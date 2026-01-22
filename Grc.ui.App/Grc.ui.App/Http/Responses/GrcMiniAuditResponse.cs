using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcMiniAuditResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("auditName")]
        public string AuditName { get; set; }

    }

}
