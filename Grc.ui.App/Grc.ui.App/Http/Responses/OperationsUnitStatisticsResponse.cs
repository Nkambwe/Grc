using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class OperationsUnitStatisticsResponse {

        /// <summary>
        /// Get or set total processes
        /// </summary>
        [JsonPropertyName("totalUnitProcess")]
        public UnitCountResponse TotalUnitProcess { get; set; }
        /// <summary>
        /// Get or set completed processes
        /// </summary>
        [JsonPropertyName("completedProcesses")]
        public UnitCountResponse CompletedProcesses { get; set; }
        /// <summary>
        /// Get or set newly proposed processes
        /// </summary>
        [JsonPropertyName("proposedProcesses")]
        public UnitCountResponse ProposedProcesses { get; set; }
        /// <summary>
        /// Get or set number of processes that have completed the default 2 year but remain unchanged
        /// </summary>
        [JsonPropertyName("unchangedProcesses")]
        public UnitCountResponse UnchangedProcesses { get; set; }
        /// <summary>
        /// Get or set number of processes that have completed the default 2 year and due for review
        /// </summary>
        [JsonPropertyName("processesDueForReview")]
        public UnitCountResponse ProcessesDueForReview { get; set; }
        /// <summary>
        /// Get or set dormant processes
        /// </summary>
        [JsonPropertyName("dormantProcesses")]
        public UnitCountResponse DormantProcesses { get; set; }
        /// <summary>
        /// Get or set processes that are nolonger in use
        /// </summary>
        [JsonPropertyName("cancelledProcesses")]
        public UnitCountResponse CancelledProcesses { get; set; }
        /// <summary>
        /// Get or set processes that are nolonger in use
        /// </summary>
        [JsonPropertyName("unclassifiedProcesses")]
        public UnitCountResponse UnclassifiedProcesses { get; set; }


    }
}
