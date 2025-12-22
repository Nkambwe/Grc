using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class RegulatoryCategoryRequest {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }
        
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
