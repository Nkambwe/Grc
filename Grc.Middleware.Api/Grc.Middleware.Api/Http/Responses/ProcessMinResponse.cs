using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ProcessMinResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("isAssigned")]
        public bool IsAssigned { get; set; }
    }
}
