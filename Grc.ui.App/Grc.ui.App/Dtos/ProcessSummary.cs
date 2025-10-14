using Grc.ui.App.Enums;

namespace Grc.ui.App.Dtos {
    public class ProcessSummary {
        public ProcessCategories Status { get; set; } = ProcessCategories.Unclassified;
        public Dictionary<OperationUnit, int> Categories { get; set; } = new();
    }
}
