namespace Grc.ui.App.Dtos {
    public class UserSupportViewModel {
        public List<BranchItemViewModel> Branches { get; set; } = new();
        public List<RoleItemViewModel> Roles { get; set; } = new();
        public List<DepartmentItemViewModel> Departments { get; set; } = new();
    }
}
