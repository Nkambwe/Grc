using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class SystemConfigurationRequest
    {
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

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime? CreatedOn { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime? ModifiedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }
    }
}
