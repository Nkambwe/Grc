using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class ExceptionReport {
        [JsonPropertyName("audit")]
        public string Audit { get; set; }

        [JsonPropertyName("auditType")]
        public string AuditType { get; set; }

        [JsonPropertyName("auditDate")]
        public DateTime AuditDate { get; set; }

        [JsonPropertyName("findings")]
        public int Findings { get; set; }

        [JsonPropertyName("closed")]
        public int Closed { get; set; }

        [JsonPropertyName("open")]
        public int Open { get; set; }

        [JsonPropertyName("percentageCompleted")]
        public double PercentageCompleted { get; set; }

        [JsonPropertyName("percentageOutstanding")]
        public double PercentageOutstanding { get; set; }

        [JsonPropertyName("averageAgingDays")]
        public int AverageAgingDays { get; set; }
    }
}
