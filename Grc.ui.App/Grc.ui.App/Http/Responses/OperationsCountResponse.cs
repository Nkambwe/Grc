using Grc.ui.App.Enums;

namespace Grc.ui.App.Http.Responses {
    public class OperationsCountResponse {
        public Dictionary<OperationUnit, int> CompletedProcesses { get; set; }
        /// <summary>
        /// Get or set newly proposed processes
        /// </summary>
        public Dictionary<OperationUnit, int> ProposedProcesses { get; set; }
        /// <summary>
        /// Get or set number of processes that have completed the default 2 year but remain unchanged
        /// </summary>
        public Dictionary<OperationUnit, int> UnchangedProcesses { get; set; }
        /// <summary>
        /// Get or set number of processes that have completed the default 2 year and due for review
        /// </summary>
        public Dictionary<OperationUnit, int> ProcessesDueForReview { get; set; }
        /// <summary>
        /// Get or set dormant processes
        /// </summary>
        public Dictionary<OperationUnit, int> DormantProcesses { get; set; }
        /// <summary>
        /// Get or set processes that are nolonger in use
        /// </summary>
        public Dictionary<OperationUnit, int> CancelledProcesses { get; set; }

        /// <summary>
        /// Get or set unit total process
        /// </summary>
        public Dictionary<ProcessCategories, int> UnitTotalProcesses { get; set; }
    }
}
