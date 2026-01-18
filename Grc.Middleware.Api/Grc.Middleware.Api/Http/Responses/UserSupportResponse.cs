namespace Grc.Middleware.Api.Http.Responses {
    public class UserSupportResponse {
        public List<BranchItemResponse> Branches {get;set;}
        public List<RoleItemResponse> Roles {get;set;}
        public List<DepartmentItemResponse> Departments {get;set;}
    }
}
