
namespace Grc.ui.App.Models {
    public class PolicyRegisterViewModel {
        public List<FrequencyViewModel> Frequencies { get; set; } = new();
        public List<DepartmentViewModel> Departments { get; set; } = new();
        public List<AuthorityViewModel> Authorities { get; set; } = new();
        public List<ResponsibilityViewModel> Responsibilities { get; set; } = new();
        public List<RegulatoryTypeViewModel> RegulatoryTypes { get; set; } = new();
        public List<StatuteMinViewModel> EnforcementLaws { get; set; } = new();
        public List<ReturnTypeViewModel> ReturnTypes { get; set; } = new();
    }
}
