using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class WorkspaceResponse {
        public CurrentUserResponse CurrentUser { get; set; }
        public IEnumerable<string> Permissions { get; set; }
        public long RoleId { get; set; }
        public string Role { get; set; }
        public AssignedBranchResponse AssignedBranch { get; set; }
        public PreferenceResponse Preferences { get; set; }
        public List<UserViewResponse> UserViews { get; set; }
    }

    public class UserViewResponse {
        public long Id { get; set; }
        public string Name { get; set; }
        public object View { get; set; }
    }

}
