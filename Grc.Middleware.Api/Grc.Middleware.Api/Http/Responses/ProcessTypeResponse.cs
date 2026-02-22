using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ProcessTypeResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("typeName")]
        public string TypeName { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }

}
