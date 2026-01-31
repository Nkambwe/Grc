using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcSystemConfigurationRequest {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("paramName")]
        public string ParamName { get; set; }

        [JsonPropertyName("paramValue")]
        public string ParamValue { get; set; }

        [JsonPropertyName("paramType")]
        public string ParamType { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }

    }

}
