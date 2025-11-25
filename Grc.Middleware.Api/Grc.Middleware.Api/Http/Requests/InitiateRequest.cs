using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class InitiateRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processStatus")]
        public string ProcessStatus { get; set; }

        [JsonPropertyName("unlockReason")]
        public string UnlockReason { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

    }

}
