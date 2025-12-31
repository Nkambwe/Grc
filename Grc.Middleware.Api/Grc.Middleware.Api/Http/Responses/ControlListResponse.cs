using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ControlListResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("controlName")]
        public string ControlName { get; set; }

    }
}
