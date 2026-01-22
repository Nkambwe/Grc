using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class MiniAuditResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("auditName")]
        public string AuditName { get; set; }

    }
}
