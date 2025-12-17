using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class FrequencyResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("frequencyName")]
        public string FrequencyName { get; set; }
    }
}
