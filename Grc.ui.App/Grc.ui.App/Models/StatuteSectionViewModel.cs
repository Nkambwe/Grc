using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class StatuteSectionViewModel
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

        [JsonPropertyName("isMandatory")]
        public bool IsMandatory { get; set; }

        [JsonPropertyName("exclude")]
        public bool ExcludeFromCompliance { get; set; }

        [JsonPropertyName("coverage")]
        public decimal Coverage { get; set; }

        [JsonPropertyName("sCovered")]
        public bool IsCovered { get; set; }

        [JsonPropertyName("assurance")]
        public decimal ComplianceAssurance { get; set; }

        [JsonPropertyName("frequencyId")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("reviewFrequency")]
        public string ReviewFrequency { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("comments")]
        public decimal Comments { get; set; }

    }
}
