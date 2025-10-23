using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class ResponsebilityRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("contactName")]
        public string ContactName { get; set; }

        [JsonPropertyName("contactPhone")]
        public string ContactPhone { get; set; }
        [JsonPropertyName("contactEmail")]
        public string ContactEmail { get; set; }

        [JsonPropertyName("contactPosition")]
        public string ContactPosition { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("ceatedOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
