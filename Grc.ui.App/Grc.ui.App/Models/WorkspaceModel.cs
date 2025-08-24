namespace Grc.ui.App.Models {

    /// <summary>
    /// Represents user workspace data containing all user-specific information.
    /// </summary>
    public class WorkspaceModel {
        public bool IsLiveEnvironment { get; set; }
        public CurrentUserModel CurrentUser { get; set; }
        public IEnumerable<string> Permissions { get; set; }
        public long RoleId { get; set; }
        public string Role { get; set; }
        public BranchModel AssignedBranch { get; set; }
        public UserPreferenceModel Preferences { get; set; }
        public List<UserViewModel> UserViews { get; set; }
    
        // Helper methods
        public bool HasPermission(string permissionName) {
            return Permissions?.Contains(permissionName) ?? false;
        }
    
        public bool HasRole(string roleName) {
            return string.Equals(Role, roleName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
