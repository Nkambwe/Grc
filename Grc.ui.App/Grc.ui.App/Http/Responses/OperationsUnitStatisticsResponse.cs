namespace Grc.ui.App.Http.Responses
{
    public class OperationsUnitStatisticsResponse {
        /// <summary>
        /// Get or set total processes
        /// </summary>
        public UnitCountResponse TotalUnitProcess { get; set; }
        /// <summary>
        /// Get or set completed processes
        /// </summary>
        public UnitCountResponse CompletedProcesses { get; set; }
        /// <summary>
        /// Get or set newly proposed processes
        /// </summary>
        public UnitCountResponse ProposedProcesses { get; set; }
        /// <summary>
        /// Get or set number of processes that have completed the default 2 year but remain unchanged
        /// </summary>
        public UnitCountResponse UnchangedProcesses { get; set; }
        /// <summary>
        /// Get or set number of processes that have completed the default 2 year and due for review
        /// </summary>
        public UnitCountResponse ProcessesDueForReview { get; set; }
        /// <summary>
        /// Get or set dormant processes
        /// </summary>
        public UnitCountResponse DormantProcesses { get; set; }
        /// <summary>
        /// Get or set processes that are nolonger in use
        /// </summary>
        public UnitCountResponse CancelledProcesses { get; set; }

        /// <summary>
        /// Get or set processes that are nolonger in use
        /// </summary>
        public UnitCountResponse UnclassifiedProcesses { get; set; }


    }
}
