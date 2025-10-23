using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class NotificationRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("sentTo")]
        public string SentTo { get; set; }

        [JsonPropertyName("carbonCopy")]
        public string CarbonCopy { get; set; }

        [JsonPropertyName("blindCopy")]
        public string BlindCopy { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("submissionId")]
        public long SubmissionId { get; set; }

        [JsonPropertyName("sentOn")]
        public DateTime SentOn { get; set; }

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
