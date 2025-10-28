namespace Grc.Middleware.Api.Http.Responses
{
    public class PermissionSetResponse {
        public long Id { get; set; }
        public string SetName { get; set; }
        public string SetDescription { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public List<PermissionResponse> Permissions { get; set; } = new();
        public List<RoleResponse> Roles { get; set; } = new();
        public List<RoleGroupResponse> RoleGroups { get; set; } = new();

    }
}
