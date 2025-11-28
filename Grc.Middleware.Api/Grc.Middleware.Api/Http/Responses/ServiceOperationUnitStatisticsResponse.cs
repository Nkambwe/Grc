using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ServiceOperationUnitStatisticsResponse {
        /// <summary>
        /// Get or set total processes
        /// </summary>
        [JsonPropertyName("totalUnitProcess")]
        public ServiceUnitCountResponse TotalUnitProcess { get; set; }
        /// <summary>
        /// Get or set completed processes
        /// </summary>
        [JsonPropertyName("completedProcesses")]
        public ServiceUnitCountResponse CompletedProcesses { get; set; }
        /// <summary>
        /// Get or set newly proposed processes
        /// </summary>
        [JsonPropertyName("proposedProcesses")]
        public ServiceUnitCountResponse ProposedProcesses { get; set; }
        /// <summary>
        /// Get or set number of processes that have completed the default 2 year but remain unchanged
        /// </summary>
        [JsonPropertyName("unchangedProcesses")]
        public ServiceUnitCountResponse UnchangedProcesses { get; set; }
        /// <summary>
        /// Get or set number of processes that have completed the default 2 year and due for review
        /// </summary>
        [JsonPropertyName("processesDueForReview")]
        public ServiceUnitCountResponse ProcessesDueForReview { get; set; }
        /// <summary>
        /// Get or set dormant processes
        /// </summary>
        [JsonPropertyName("dormantProcesses")]
        public ServiceUnitCountResponse DormantProcesses { get; set; }
        /// <summary>
        /// Get or set processes that are nolonger in use
        /// </summary>
        [JsonPropertyName("cancelledProcesses")]
        public ServiceUnitCountResponse CancelledProcesses { get; set; }

        /// <summary>
        /// Get or set processes that are nolonger in use
        /// </summary>
        [JsonPropertyName("unclassifiedProcesses")]
        public ServiceUnitCountResponse UnclassifiedProcesses { get; set; }
    }
}
