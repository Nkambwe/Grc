
namespace Grc.ui.App.Models {
    public class StatutoryViewModel {
        public List<FrequencyViewModel> Frequencies { get; set; } = new();
        public List<AuthorityViewModel> Authorities { get; set; } = new();
        public List<StatuteTypeViewModel> StatuteTypes { get; set; } = new();
        public List<DepartmentViewModel> Departments { get; set; } = new();
        public List<ResponsibilityViewModel> Responsibilities { get; set; } = new();
    }
}
