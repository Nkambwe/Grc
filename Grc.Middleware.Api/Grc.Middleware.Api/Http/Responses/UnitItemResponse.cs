using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class UnitItemResponse
    {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("unitName")]
        public string UnitName { get; set; }
    }
}
