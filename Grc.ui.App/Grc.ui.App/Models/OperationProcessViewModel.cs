namespace Grc.ui.App.Models {
    public class OperationProcessViewModel
    {
        public List<ProcessTypeViewModel> ProcessTypes { get; set; } = new();
        public List<ProcessStatusViewModel> ProcessStatuses { get; set; } = new();
        public List<UnitViewModel> Units { get; set; } = new();

        public List<ResponsibilityViewModel> Responsibilities { get; set; } = new();
    }

}
