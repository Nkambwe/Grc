
namespace Grc.ui.App.Dtos {
    public class AuditStatistic {
        /// <summary>
        /// Get/Set number of findings to be completed, less than a month, two-three months, above three months
        /// </summary>
        public Dictionary<string, int> Findings { get; set; }

        /// <summary>
        /// Get/Set findings per audit
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> BarChart { get; set; }

        /// <summary>
        /// Get/Set findings by status
        /// </summary>
        public Dictionary<string, int> Completions { get; set; }
    }
}
