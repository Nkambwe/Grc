namespace Grc.Middleware.Api.Http.Responses {
    public class WorkspaceResponse {
        public CurrentUserResponse CurrentUser { get; set; }
        public IEnumerable<string> Permissions { get; set; }
        public long RoleId { get; set; }
        public string Role { get; set; }
        public AssignedBranchResponse AssignedBranch { get; set; }
        public PreferenceResponse Preferences { get; set; }
        public List<UserViewResponse> UserViews { get; set; }
    }

}
