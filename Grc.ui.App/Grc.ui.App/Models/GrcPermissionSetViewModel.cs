namespace Grc.ui.App.Models {
    public class GrcPermissionSetViewModel
    {
        public long Id { get; set; }
        public string SetName { get; set; }
        public string Description { get; set; }
        public List<long> Roles { get; set; } = new();
        public List<long> Permissions { get; set; } = new();
        public List<long> RoleGroups { get; set; } = new();
    }
}
