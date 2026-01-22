using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class AuditMiniTypeResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("typeName")]
        public string TypeName { get; set; }

    }
}
