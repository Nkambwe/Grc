using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcStatutorySectionResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("statutoryId")]
        public long StatutoryId { get; set; }

        [JsonPropertyName("section")]
        public string Section { get; set; }

        [JsonPropertyName("summery")]
        public string Summery { get; set; }

        [JsonPropertyName("obligation")]
        public string Obligation { get; set; }

        [JsonPropertyName("frequencyId")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("frequency")]
        public string ObligationFrequency { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("isMandatory")]
        public bool IsMandatory { get; set; }

        [JsonPropertyName("exclude")]
        public bool ExcludeFromCompliance { get; set; }

        [JsonPropertyName("coverage")]
        public decimal Coverage { get; set; }

        [JsonPropertyName("isCovered")]
        public bool IsCovered { get; set; }

        [JsonPropertyName("assurance")]
        public decimal ComplianceAssurance { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("revisions")]
        public List<GrcArticleRevisionResponse> Revisions { get; set; } = new();
    }
}
