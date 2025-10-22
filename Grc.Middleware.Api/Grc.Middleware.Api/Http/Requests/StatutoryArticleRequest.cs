using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class StatutoryArticleRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("article")]
        public string Article { get; set; }

        [JsonPropertyName("summery")]
        public string Summery { get; set; }

        [JsonPropertyName("obligation")]
        public string ObligationOrRequirement { get; set; }

        [JsonPropertyName("statuteId")]
        public long StatuteId { get; set; }

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
